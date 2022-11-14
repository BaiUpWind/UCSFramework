using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Net;
using System.Reflection;
using System.Linq;

using MSWord = Microsoft.Office.Interop.Word;


namespace ConsoleTest
{
    internal class Program
    {

         
        public readonly static string SysPath = Directory.GetCurrentDirectory().ToString();
        public readonly static string SavePath = SysPath  +@"\img\";
        public readonly static string DbPath = SysPath + @"\sesedb";
        public readonly static Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

        public readonly static string DicPath = @"D:\Code\UCSFramework\ConsoleTest\bin\Debug\img\20221114_194108";


        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            var infos = GetAllFileInfo(new DirectoryInfo(DicPath), new List<FileInfo>());

            //PDF(SavePath + "\\A1.pdf", infos.Select(a => a.FullName).ToList());

            object path = SavePath + "A1.doc";

            SaveToDoc(path, infos.Select(a => a.FullName).ToList());

            Console.WriteLine("ok");
            Console.ReadKey();

            return;
            SQLiteConnection connection = null;
            try
            {
                connection = new SQLiteConnection("data source = " + DbPath);
                connection.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"数据库打开失败! {ex.Message}");
                Console.ReadKey();
                return;
            }
            Console.WriteLine("数据库打开成功!");

            var timestampStr = config.AppSettings.Settings["timestamp"].Value;
            var maxcount = config.AppSettings.Settings["maxcount"].Value;
            var channelid = config.AppSettings.Settings["channelid"].Value;
            if (string.IsNullOrWhiteSpace(timestampStr))
            {
                timestampStr = "0";
            }
            var dirName = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var dicPath = SavePath + $"{dirName}"  ;
            if (!Directory.Exists(dicPath))
            {
                Directory.CreateDirectory(dicPath);
            }

            if (!long.TryParse(timestampStr, out long timestamp)) return;

            string sql = @"select a.name,a.size,a.url,a.type,b.timestamp from attachments a join messages b  on a.message_id = b.message_id
                           where b.timestamp > "+ timestamp + " and a.type like '%image%'  order by b.timestamp  limit " + maxcount + "";

            sql = $@"select a.name,a.size,a.url,a.type,b.timestamp from attachments a, messages b ,channels c
                where a.message_id = b.message_id and b.channel_id = c.id and b.timestamp > {timestamp} and substr(a.type,0,6) = 'image' 
                and a.size < 1048576 and c.id = '{channelid}'
                order by b.timestamp limit {maxcount}";


            SQLiteCommand cmd = new SQLiteCommand();
            SQLiteDataAdapter adapter = new SQLiteDataAdapter();
            cmd.CommandText = sql;
            cmd.Connection = connection;
            adapter.SelectCommand = cmd;
            DataSet ds = new DataSet();
            adapter.Fill(ds);
            var list = DataSetToEntityList<ImageInfo>(ds, 0);
            int failCount = 0;
            List<string> imgPaths = new List<string>();
            foreach (var item in list)
            { 
                using (WebClient webClient = new WebClient())
                {
                    try
                    {
                        var fileName = SavePath + dirName + "\\" + item.NAME;
                        webClient.DownloadFile(item.URL, fileName);
                        //Bitmap image = new Bitmap(fileName);
                        imgPaths.Add(fileName);
                        Console.WriteLine($"获取成功'{item.NAME}'");
                        failCount = 0;
                    }
                    catch (Exception)
                    {
                        if (failCount > 3)
                        {
                            Console.WriteLine($"尝试重新获取次数大于3次，跳到下一个目标进行获取");
                            continue;
                        }
                        failCount++;
                        Console.WriteLine($"错误，尝试重新获取！count'{failCount}'");
                    }
                }
            } 
            config.AppSettings.Settings["timestamp"].Value = list[list.Count - 1].TIMESTAMP.ToString();
            config.Save();
            SaveToDoc(SavePath + dirName + $"\\1A_{dirName}.doc", imgPaths);

            Console.WriteLine("Finishied,按任意键关闭!"); 
            Console.ReadKey();
        }

        /// <summary>
        /// 将多张图片合成到一个PDF内，完美填充页，页大小(mm)为图片大小(mm)
        /// </summary>
        /// <param name="输出目录">最终的PDF输出目录</param>
        /// <param name="图片路径">图片的路径集合</param>
        public static void PDF(string 输出目录, List<string> 图片路径)
        {
            Dictionary<string, float[]> dic = new Dictionary<string, float[]>();
            //获取PDF页的实际长宽(mm)
            float[] xy = new float[2];
            foreach (var item in 图片路径)
            {
                xy = new float[2];
                using (FileStream fs = new FileStream(item, FileMode.Open))
                {
                    System.Drawing.Image img = System.Drawing.Image.FromStream(fs);
                    int w = img.Width;
                    int h = img.Height;
                    float w_dpi = img.HorizontalResolution;//水平分辨率
                    float h_dpi = img.VerticalResolution;//垂直分辨率
                    xy[0] = (float)(w * 25.4 / w_dpi * 2.83462677);//PDF实际宽度(mm)要乘2.83462677
                    xy[1] = (float)(h * 25.4 / h_dpi * 2.83462677);
                    dic.Add(item, xy);
                }
            }

            //创建页属性对象，Rectangle：设置长宽，最后4个0设置左右上下边距
            iTextSharp.text.Document document = new iTextSharp.text.Document(new iTextSharp.text.Rectangle(0, 0, xy[0], xy[1]), 0, 0, 0, 0);
            using (FileStream fs = new FileStream(输出目录, FileMode.Create))
            {
                iTextSharp.text.pdf.PdfWriter.GetInstance(document, fs);//将页设置与PDF输出流合并
                document.Open();//打开PDF
                                //插入图片，一个图片占满一页
                for (int i = 0; i < 图片路径.Count; i++)
                {
                    string imgPath = 图片路径[i];
                    using (FileStream imgFs = new FileStream(imgPath, FileMode.Open))
                    {
                        iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(imgFs);//从流读取图片
                                                                                             //img.Alignment = Element.ALIGN_CENTER;//图片居中
                        if (dic.TryGetValue(imgPath, out   xy))
                        {
                            img.ScaleAbsolute(xy[0], xy[1]);//设置图片大小
                            document.NewPage();//创建新页，并指向新页
                            document.Add(img);//往新页中添加图片
                            imgFs.Close();//重要，防止内存溢出，必要时可调用GC强制等待清理
                        }
                    }
                }
                document.Close();//关闭PDF
            }
        }

        public static void SaveToDoc(object filePath, List<string> imgPaths)
        {
            MSWord.Application wordApp;                   //Word应用程序变量
            MSWord.Document wordDoc;                  //Word文档变量
            wordApp = new MSWord.Application(); //初始化


            //wordApp.Visible = true;//使文档可见
            //如果已存在，则删除
            if (File.Exists(filePath.ToString()))
            {
                File.Delete(filePath.ToString());
            }
            Object Nothing = Missing.Value;
            wordDoc = wordApp.Documents.Add(ref Nothing, ref Nothing, ref Nothing, ref Nothing);
            wordDoc.PageSetup.PaperSize = MSWord.WdPaperSize.wdPaperA4;//设置纸张样式为A4纸
            wordDoc.PageSetup.Orientation = MSWord.WdOrientation.wdOrientPortrait;//排列方式为垂直方向
            wordDoc.PageSetup.TopMargin = 57.0f;
            wordDoc.PageSetup.BottomMargin = 57.0f;
            wordDoc.PageSetup.LeftMargin = 57.0f;
            wordDoc.PageSetup.RightMargin = 57.0f;
            wordDoc.PageSetup.HeaderDistance = 30.0f;//页眉位置

            foreach (var item in imgPaths)
            {
                Object range = wordDoc.Paragraphs.Last.Range;
                //定义该插入的图片是否为外部链接
                Object linkToFile = false;               //默认,这里貌似设置为bool类型更清晰一些
                                                         //定义要插入的图片是否随Word文档一起保存
                Object saveWithDocument = true;              //默认
                                                             //使用InlineShapes.AddPicture方法(【即“嵌入型”】)插入图片
                wordDoc.InlineShapes.AddPicture(item, ref linkToFile, ref saveWithDocument, ref range);
                wordApp.Selection.ParagraphFormat.Alignment = MSWord.WdParagraphAlignment.wdAlignParagraphCenter;//居中显示图片
            }
            object format = MSWord.WdSaveFormat.wdFormatDocument;// office 2007就是wdFormatDocumentDefault
         

          var a =  wordDoc.BuiltInDocumentProperties(MSWord.WdBuiltInProperty.wdPropertyLastAuthor) ;
            a = null;
            //将wordDoc文档对象的内容保存为DOCX文档
            wordDoc.SaveAs(ref filePath, ref format, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing);
            wordDoc.Close(ref Nothing, ref Nothing, ref Nothing);


            //关闭wordApp组件对象
            wordApp.Quit(ref Nothing, ref Nothing, ref Nothing);
        }

 

        static List<System.IO.FileInfo> GetAllFileInfo(System.IO.DirectoryInfo dir,List<FileInfo> FileList)
        {
            System.IO.FileInfo[] allFile = dir.GetFiles();
            foreach (System.IO.FileInfo file in allFile)
            {
                FileList.Add(file);
            }
            System.IO.DirectoryInfo[] allDir = dir.GetDirectories();
            foreach (System.IO.DirectoryInfo d in allDir)
            {
                GetAllFileInfo(d, FileList);
            }
            return FileList;
        }
        /// <summary>
        /// DataSet转换为实体列表
        /// </summary>
        /// <typeparam name="T">实体类</typeparam>
        /// <param name="p_DataSet">DataSet</param>
        /// <param name="p_TableIndex">待转换数据表索引</param>
        /// <returns>实体类列表</returns>
        public static IList<T> DataSetToEntityList<T>(DataSet p_DataSet, int p_TableIndex)
        {
            if (p_DataSet == null || p_DataSet.Tables.Count < 0)
                return default(IList<T>);
            if (p_TableIndex > p_DataSet.Tables.Count - 1)
                return default(IList<T>);
            if (p_TableIndex < 0)
                p_TableIndex = 0;
            if (p_DataSet.Tables[p_TableIndex].Rows.Count <= 0)
                return default(IList<T>);
            DataTable p_Data = p_DataSet.Tables[p_TableIndex];
            // 返回值初始化
            IList<T> result = new List<T>();
            for (int j = 0; j < p_Data.Rows.Count; j++)
            {
                T _t = (T)Activator.CreateInstance(typeof(T));
                PropertyInfo[] propertys = _t.GetType().GetProperties();
                foreach (PropertyInfo pi in propertys)
                {
                    if (p_Data.Columns.IndexOf(pi.Name.ToUpper()) != -1 && p_Data.Rows[j][pi.Name.ToUpper()] != DBNull.Value)
                    {
                        pi.SetValue(_t, p_Data.Rows[j][pi.Name.ToUpper()], null);
                    }
                    else
                    {
                        pi.SetValue(_t, null, null);
                    }
                }
                result.Add(_t);
            }
            return result;
        }
        public class ImageInfo
        {
            public string NAME { get; set; }
            public long SIZE { get; set; }
            public string URL { get; set; }
            public long TIMESTAMP { get; set; }
        }
    }
}
