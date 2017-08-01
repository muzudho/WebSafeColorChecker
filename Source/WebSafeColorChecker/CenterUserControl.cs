using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Collections.Generic;

namespace WebSafeColorCheck
{
    public partial class CenterUserControl : UserControl
    {
        public CenterUserControl()
        {
            InitializeComponent();
        }

        const string RESULT_YES_NO_SUFFIX = "_YesNo_result.bmp";
        const string RESULT_TRANSPARENT_SUFFIX = "_Transparent_result.bmp";
        const string RESULT_HTML = "result.html";

        string[] filePaths;
        List<Bitmap> bitmaps;
        Color yesColor = Color.FromArgb(255, 204, 204, 204);
        Color noColor = Color.FromArgb(255, 204, 51, 102);

        /// <summary>
        /// [実行]
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            // ファイル名検索
            filePaths = Directory.GetFiles(".");
            bitmaps = new List<Bitmap>();
            foreach (string path in filePaths)
            {
                if (
                    path.EndsWith(RESULT_YES_NO_SUFFIX) ||
                    path.EndsWith(RESULT_TRANSPARENT_SUFFIX)
                    )
                {
                    bitmaps.Add(null);
                }
                else
                {
                    try
                    {
                        bitmaps.Add(new Bitmap(Image.FromFile(path)));
                    }
                    catch (Exception)
                    {
                        bitmaps.Add(null);
                    }
                }
            }

            int i = 0;
            foreach (Bitmap bmp in bitmaps)
            {
                if (null != bmp)
                {
                    WriteYesNoImage(i, bmp);
                    WriteTransparentImage(i, bmp);
                }

                i++;
            }

            // HTML出力
            WriteHtml();
        }

        void WriteYesNoImage(int i, Bitmap bmp)
        {
            Bitmap bitmapRet = new Bitmap(bmp.Width, bmp.Height);

            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    if (IsWebSafeColor(bmp.GetPixel(x, y)))
                    {
                        bitmapRet.SetPixel(x, y, yesColor);
                    }
                    else
                    {
                        bitmapRet.SetPixel(x, y, noColor);
                    }
                }
            }
            bitmapRet.Save(filePaths[i] + RESULT_YES_NO_SUFFIX);
        }
        void WriteTransparentImage(int i, Bitmap bmp)
        {
            Bitmap bitmapRet = new Bitmap(bmp.Width, bmp.Height);

            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    Color color = bmp.GetPixel(x, y);
                    int value = color.A;
                    bitmapRet.SetPixel(x, y, Color.FromArgb(255, value, value, value));
                }
            }
            bitmapRet.Save(filePaths[i] + RESULT_TRANSPARENT_SUFFIX);
        }

        /// <summary>
        /// Webセーフカラーか
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        bool IsWebSafeColor(Color color)
        {
            int[] rgb = new int[]
            {
                color.R,
                color.G,
                color.B,
            };

            foreach (int value in rgb)
            {
                if (value != 0 &&
                    value != 51 &&
                    value != 102 &&
                    value != 153 &&
                    value != 204 &&
                    value != 255)
                {
                    return false;
                }
            }

            return true;
        }

        void WriteHtml()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(@"<!DOCTYPE html>
<html>
    <head>
        <!-- Bootstrap CSS -->
        <link rel=""stylesheet"" href=""./bootstrap.min.css"">
        <title>Webセーフカラー・チェッカー</title>
    </head>
    <body>
        <h1>Webセーフカラー・チェッカー</h1>
        <span style=""display:inline-block; background-color:#CCC; width:20px;"">&nbsp;</span>YES<br />
        <span style=""display:inline-block; background-color:#C36; width:20px;"">&nbsp;</span>NO<br />
        <br />
        <table class=""table table-bordered"">
            <tr>
                <td></td><td>元画像</td><td>セーフカラー・チェック</td><td>透明度チェック</td>
            </tr>
");

            int i=0;
            foreach (Bitmap bmp in bitmaps)
            {
                if (null != bmp)
                {
                    sb.Append(@"        <tr>
            <td>" + filePaths[i] + @"</td><td><img src=""" + filePaths[i] + @""" /></td><td><img src=""" + (filePaths[i] + RESULT_YES_NO_SUFFIX) + @""" /></td><td><img src=""" + (filePaths[i] + RESULT_TRANSPARENT_SUFFIX) + @""" style=""border:solid 1px Green;"" /></td>
        </tr>
");
                }
                i++;
            }

            sb.Append(@"        </table>
    </body>
</html>
");

            File.WriteAllText(RESULT_HTML, sb.ToString());
        }
    }
}
