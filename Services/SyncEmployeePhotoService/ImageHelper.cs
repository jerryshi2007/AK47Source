using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace SyncEmployeePhotoService
{
    /// <summary>
    /// 图片处理类
    /// 1、生成缩略图片或按照比例改变图片的大小和画质
    /// 2、将生成的缩略图放到指定的目录下
    /// </summary>
    public class ImageHelper
    {
        public Image ResourceImage;
        private int ImageWidth;
        private int ImageHeight;

        public string ErrMessage;

        /// <summary>
        /// 类的构造函数
        /// </summary>
        /// <param name="ImageFileName">图片文件的全路径名称</param>
        public ImageHelper(byte[] images)
        {
            MemoryStream ms = new MemoryStream(images);
            ResourceImage = Image.FromStream(ms);
            ms.Close();
        }

        public bool ThumbnailCallback()
        {
            return false;
        }

        /// <summary>
        /// 生成缩略图重载方法1，返回缩略图的Image对象
        /// </summary>
        /// <param name="Width">缩略图的宽度</param>
        /// <param name="Height">缩略图的高度</param>
        /// <returns>缩略图的Image对象</returns>
        public Image GetReducedImage(int Width, int Height)
        {
            try
            {
                Image ReducedImage;

                Image.GetThumbnailImageAbort callb = new Image.GetThumbnailImageAbort(ThumbnailCallback);

                ReducedImage = ResourceImage.GetThumbnailImage(Width, Height, callb, IntPtr.Zero);

                return ReducedImage;
            }
            catch (Exception e)
            {
                ErrMessage = e.Message;
                return null;
            }
        }

        /// <summary>
        /// 生成缩略图重载方法2，将缩略图文件保存到指定的路径
        /// </summary>
        /// <param name="Width">缩略图的宽度</param>
        /// <param name="Height">缩略图的高度</param>
        /// <param name="targetFilePath">缩略图保存的全文件名，(带路径)，参数格式：D:Imagesfilename.jpg</param>
        /// <returns>成功返回true，否则返回false</returns>
        public bool GetReducedImage(int Width, int Height, string targetFilePath)
        {
            try
            {
                Image ReducedImage;

                Image.GetThumbnailImageAbort callb = new Image.GetThumbnailImageAbort(ThumbnailCallback);

                ReducedImage = ResourceImage.GetThumbnailImage(Width, Height, callb, IntPtr.Zero);
                ReducedImage.Save(@targetFilePath, ImageFormat.Jpeg);

                ReducedImage.Dispose();

                return true;
            }
            catch (Exception e)
            {
                ErrMessage = e.Message;
                return false;
            }
        }

        /// <summary>
        /// 生成缩略图重载方法3，返回缩略图的Image对象
        /// </summary>
        /// <param name="Percent">缩略图的宽度百分比如：需要百分之80，就填0.8</param> 
        /// <returns>缩略图的Image对象</returns>
        public Image GetReducedImage(double Percent)
        {
            try
            {
                Image ReducedImage;

                Image.GetThumbnailImageAbort callb = new Image.GetThumbnailImageAbort(ThumbnailCallback);

                ImageWidth = Convert.ToInt32(ResourceImage.Width * Percent);
                ImageHeight = Convert.ToInt32(ResourceImage.Width * Percent);

                ReducedImage = ResourceImage.GetThumbnailImage(ImageWidth, ImageHeight, callb, IntPtr.Zero);

                return ReducedImage;
            }
            catch (Exception e)
            {
                ErrMessage = e.Message;
                return null;
            }
        }

        /// <summary>
        /// 生成缩略图重载方法4，返回缩略图的Image对象
        /// </summary>
        /// <param name="Percent">缩略图的宽度百分比如：需要百分之80，就填0.8</param> 
        /// <param name="targetFilePath">缩略图保存的全文件名，(带路径)，参数格式：D:Imagesfilename.jpg</param>
        /// <returns>成功返回true,否则返回false</returns>
        public bool GetReducedImage(double Percent, string targetFilePath)
        {
            try
            {
                Image ReducedImage;

                Image.GetThumbnailImageAbort callb = new Image.GetThumbnailImageAbort(ThumbnailCallback);

                ImageWidth = Convert.ToInt32(ResourceImage.Width * Percent);
                ImageHeight = Convert.ToInt32(ResourceImage.Width * Percent);

                ReducedImage = ResourceImage.GetThumbnailImage(ImageWidth, ImageHeight, callb, IntPtr.Zero);
                ReducedImage.Save(@targetFilePath, ImageFormat.Jpeg);
                ReducedImage.Dispose();

                return true;
            }
            catch (Exception e)
            {
                ErrMessage = e.Message;
                return false;
            }
        }

        /// <summary>
        /// 将image数组转换成byte[]
        /// </summary>
        /// <param name="byteArrayIn"></param>
        /// <returns></returns>
        public byte[] ImageToArray(System.Drawing.Image image)
        {
            MemoryStream ms = new MemoryStream();
            byte[] imagedata = null;
            image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            imagedata = ms.GetBuffer();
            ms.Close();
            return imagedata;

        }

        /// <summary>
        /// 将byte[]数组转换成image
        /// </summary>
        /// <param name="byteArrayIn"></param>
        /// <returns></returns>
        public Image byteArrayToImage(byte[] byteArrayIn)
        {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            Image returnImage = Image.FromStream(ms);
            ms.Close();
            return returnImage;
        }

        /// <summary>
        /// 析构函数
        /// </summary>
         ~ImageHelper()
        {
            ResourceImage.Dispose();
        }

         /**/
         /// <param name="SavePath">原始路径</param>
         /// <param name="pathDate">添加的月份路径</param>
         /// <param name="picFilePath">保存的文件名</param>
         /// <param name="width">宽</param>
         /// <param name="height">高</param>
         public byte[] GetThumbnailImage(string fileExtension, double percent)
         {
             Bitmap img = new Bitmap(this.ResourceImage); //read picture to memory

             int h = img.Height;
             int w = img.Width;
             int ss, os;// source side and objective side
             double temp1, temp2;
             //compute the picture's proportion
             temp1 = (h * 1.0D) / (h * percent);
             temp2 = (w * 1.0D) / (w * percent);
             if (temp1 < temp2)
             {
                 ss = w;
                 os = (int)(w * percent);
             }
             else
             {
                 ss = h;
                 os = (int)(h * percent);
             }

             double per = (os * 1.0D) / ss;
             if (per < 1.0D)
             {
                 h = (int)(h * per);
                 w = (int)(w * per);
             }
             // create the thumbnail image
             System.Drawing.Image imag2 = img.GetThumbnailImage(w, h,
                  new System.Drawing.Image.GetThumbnailImageAbort(ThumbnailCallback),
                  IntPtr.Zero);
             Bitmap tempBitmap = new Bitmap(w, h);
             System.Drawing.Image tempImg = System.Drawing.Image.FromHbitmap(tempBitmap.GetHbitmap());
             System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(tempImg);
             g.Clear(Color.White);

             int x, y;

             x = (tempImg.Width - imag2.Width) / 2;
             y = (tempImg.Height - imag2.Height) / 2;

             g.DrawImage(imag2, x, y, imag2.Width, imag2.Height);

             try
             {
                 if (img != null)
                     img.Dispose();
                 if (imag2 != null)
                     imag2.Dispose();
                 if (tempBitmap != null)
                     tempBitmap.Dispose();

                 MemoryStream ms = new MemoryStream();
                 byte[] imagedata = null;
                 //按原图片类型保存缩略图片,不按原格式图片会出现模糊,锯齿等问题. 
                 switch (fileExtension)
                 {
                     case "gif": tempImg.Save(ms, System.Drawing.Imaging.ImageFormat.Gif); break;
                     case "jpg": tempImg.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);break;
                     case "bmp": tempImg.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp); break;
                     case "png": tempImg.Save(ms, System.Drawing.Imaging.ImageFormat.Png); break;
                 }

                 imagedata = ms.GetBuffer();
                 ms.Close();
                 return imagedata;
             }
             catch
             {
                 throw new Exception("图片上传失败");
             }
             finally
             {
                 //释放内存
                 if (tempImg != null)
                     tempImg.Dispose();
                 if (g != null)
                     g.Dispose();
             }
         }
    }
}

