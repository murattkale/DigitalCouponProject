using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


public class ImageProcess
{


  


    public Bitmap ResizeImage(Image image, int width, int height)
    {
        if (image.Width <= width)
        {
            width = image.Width;
        }
        if (image.Height <= height)
        {
            height = image.Height;
        }
        Bitmap bmpt = new Bitmap(width, height);
        Graphics grt = Graphics.FromImage(bmpt);
        grt.CompositingQuality = CompositingQuality.Default;
        grt.SmoothingMode = SmoothingMode.Default;
        grt.InterpolationMode = InterpolationMode.Bicubic;
        grt.PixelOffsetMode = PixelOffsetMode.Default;
        grt.DrawImage(image, 0, 0, width, height);
        return bmpt;
    }
    public Bitmap ResizeImageFixedWidth(Image image, int width)
    {
        int srcWidth = image.Width;
        int srcHeight = image.Height;
        int thumbWidth = 0;
        int thumbHeight = 0;
        if (srcHeight > srcWidth)
        {
            double horan = (srcWidth * 100) / srcHeight;
            thumbHeight = width;
            thumbWidth = Convert.ToInt32((thumbHeight * horan) / 100);
        }
        else if (srcWidth > srcHeight)
        {
            double woran = (srcHeight * 100) / srcWidth;
            thumbWidth = width;
            thumbHeight = Convert.ToInt32((thumbWidth * woran) / 100);
        }
        else if (srcHeight == srcWidth)
        {
            thumbHeight = width;
            thumbWidth = width;
        }
        Bitmap bmp = new Bitmap(thumbWidth, thumbHeight);
        bmp.SetResolution(image.HorizontalResolution, image.VerticalResolution);
        Graphics gr = Graphics.FromImage(bmp);
        gr.CompositingQuality = CompositingQuality.Default;
        gr.SmoothingMode = SmoothingMode.Default;
        gr.InterpolationMode = InterpolationMode.Bicubic;
        gr.PixelOffsetMode = PixelOffsetMode.Default;
        Rectangle imageRectangle = new Rectangle(0, 0, thumbWidth, thumbHeight);
        Rectangle rectDestination = new Rectangle(0, 0, thumbWidth, thumbHeight);
        gr.DrawImage(image, imageRectangle, rectDestination, GraphicsUnit.Pixel);
        gr.Dispose();
        return bmp;
    }
    //public void CropImageAndSave(Image image, string url, int x, int y, int width, int height)
    //{
    //    Rectangle rectDestination = new Rectangle(x, y, width, height);
    //    Bitmap bmp = new Bitmap(rectDestination.Width, rectDestination.Height);
    //    Graphics gr = Graphics.FromImage(bmp);
    //    gr.CompositingQuality = CompositingQuality.Default;
    //    gr.SmoothingMode = SmoothingMode.Default;
    //    gr.InterpolationMode = InterpolationMode.Bicubic;
    //    gr.PixelOffsetMode = PixelOffsetMode.Default;
    //    gr.DrawImage(image, new Rectangle(0, 0, bmp.Width, bmp.Height), rectDestination, GraphicsUnit.Pixel);
    //    bmp.Save(HttpContext.Current.Server.MapPath(url), image.RawFormat);
    //    bmp.Dispose();
    //    image.Dispose();
    //}
    public Bitmap CropImage(Image image, int x, int y, int width, int height)
    {
        Rectangle rectDestination = new Rectangle(x, y, width, height);
        Bitmap bmp = new Bitmap(rectDestination.Width, rectDestination.Height);
        Graphics gr = Graphics.FromImage(bmp);
        gr.CompositingQuality = CompositingQuality.Default;
        gr.SmoothingMode = SmoothingMode.Default;
        gr.InterpolationMode = InterpolationMode.Bicubic;
        gr.PixelOffsetMode = PixelOffsetMode.Default;
        gr.DrawImage(image, new Rectangle(0, 0, bmp.Width, bmp.Height), rectDestination, GraphicsUnit.Pixel);
        return bmp;
    }
    public static Bitmap WatermarkImage(Bitmap image, Bitmap watermark)
    {
        using (Graphics imageGraphics = Graphics.FromImage(image))
        {
            watermark.SetResolution(imageGraphics.DpiX, imageGraphics.DpiY);
            int x = (image.Width - watermark.Width) / 2;
            int y = (image.Height - watermark.Height) / 2;
            Image _watermark = SetImageOpacity(watermark, .5f);
            imageGraphics.DrawImage(_watermark, x, y, _watermark.Width, _watermark.Height);
        }
        return image;
    }
    public static Image SetImageOpacity(Image image, float opacity)
    {
        try
        {
            //create a Bitmap the size of the image provided  
            Bitmap bmp = new Bitmap(image.Width, image.Height);
            //create a graphics object from the image  
            using (Graphics gfx = Graphics.FromImage(bmp))
            {
                //create a color matrix object  
                ColorMatrix matrix = new ColorMatrix
                {
                    //set the opacity  
                    Matrix33 = opacity
                };
                //create image attributes  
                ImageAttributes attributes = new ImageAttributes();
                //set the color(opacity) of the image  
                attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                //now draw the image  
                gfx.DrawImage(image, new Rectangle(0, 0, bmp.Width, bmp.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attributes);
            }
            return bmp;
        }
        catch (Exception)
        {
            return null;
        }
    }
    public static ImageCodecInfo GetEncoder(ImageFormat format)
    {
        ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
        foreach (ImageCodecInfo codec in codecs)
        {
            if (codec.FormatID == format.Guid)
            {
                return codec;
            }
        }
        return null;
    }
    //public static void SaveJpeg(string path, Image image)
    //{
    //    ImageCodecInfo encoder = GetEncoder(ImageFormat.Jpeg);
    //    if (encoder != null)
    //    {
    //        EncoderParameters encoderParameters = new EncoderParameters(1);
    //        encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, 50L);
    //        image.Save(path, encoder, encoderParameters);
    //    }
    //    else
    //    {
    //        image.Save(path);
    //    }
    //}
}

class ReplaceExpressionVisitor : ExpressionVisitor
{
    private readonly Expression _oldValue;
    private readonly Expression _newValue;

    public ReplaceExpressionVisitor(Expression oldValue, Expression newValue)
    {
        _oldValue = oldValue;
        _newValue = newValue;
    }

    public override Expression Visit(Expression node)
    {
        if (node == _oldValue)
        {
            return _newValue;
        }

        return base.Visit(node);
    }
}

public static class Helpers
{
    public static DateTime AyinIlkGunu(this DateTime dt)
    {
        return new DateTime(dt.Year, dt.Month, 1);
    }

    public static DateTime AyinSonGunu(this DateTime dt)
    {
        return dt.AyinIlkGunu().AddMonths(1).AddDays(-1);
    }



    public static String ConvertImageURLToBase64(this string url)
    {
        StringBuilder _sb = new StringBuilder();

        Byte[] _byte = GetImage(url);
        if (_byte != null)
        {
            _sb.Append(Convert.ToBase64String(_byte, 0, _byte.Length));
        }
        return _sb.ToString();
    }

    public static byte[] GetImage(string url)
    {
        Stream stream = null;
        byte[] buf;

        try
        {
            WebProxy myProxy = new WebProxy();
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);

            HttpWebResponse response = (HttpWebResponse)req.GetResponse();
            stream = response.GetResponseStream();

            using (BinaryReader br = new BinaryReader(stream))
            {
                int len = (int)(response.ContentLength);
                buf = br.ReadBytes(len);
                br.Close();
            }

            stream.Close();
            response.Close();
        }
        catch (Exception exp)
        {
            buf = null;
        }

        return (buf);
    }


    static Dictionary<string, string> dicSupportedFormats = new Dictionary<string, string>{
    {".jpg", "jpeg"},
    {".jpeg", "jpeg"},
    {".jfif", "jpeg"},
    {".pjp", "jpeg"},
    {".pjpeg", "jpeg"},
    {".png", "png"},
    {".svg", "svg+xml"},
    {".webp", "webp"},
    {".gif", "gif"},
    {".avif", "avif"},
    {".apng", "apng"},
    {".ico", "x-icon"},
    {".cur", "x-icon"},
    {".tiff", "tiff"},
    {".tif", "tiff"},
};

    public static string GetBase64Uri(this string imgFile)
    {
        var ext = Path.GetExtension(imgFile);
        if (dicSupportedFormats.TryGetValue(ext, out var typeStr))
        {
            var extPath = System.Text.Encoding.UTF8.GetBytes(imgFile);
            var base64 = $"<img src=\"data:image/{typeStr};base64,{Convert.ToBase64String(extPath)}\" />";
            return base64;
        }
        else
            return null;
    }

    public static string toImageToBase64(this string path)
    {
        try
        {
            var ext = Path.GetExtension(path);
            var extPath = System.Text.Encoding.UTF8.GetBytes(path);
            if (dicSupportedFormats.TryGetValue(ext, out var typeStr))
            {
                var base64 = $"data:image/{Path.GetExtension(typeStr).TrimStart('.')};base64,{Convert.ToBase64String(extPath)}";
                return base64;
            }
            else
            {
                return "";
            }

        }
        catch (Exception ex)
        {

            return "";
        }

    }

    public static string toImageToBase64_2(this string path)
    {
        var ext = Path.GetExtension(path);
        var str = ConvertImageURLToBase64(path);
        if (dicSupportedFormats.TryGetValue(ext, out var typeStr))
        {
            var base64 = $"data:image/{Path.GetExtension(typeStr).TrimStart('.')};base64,{str}";
            return base64;
        }
        else
        {
            return "";
        }
    }


    public static Image toImageToBase64Decode(this string base64String)
    {
        var img = Image.FromStream(new MemoryStream(Convert.FromBase64String(base64String)));
        return img;
    }


    public static string toRandom(string harfler)
    {

        Random rastgele = new Random();
        string uret = "";
        for (int i = 0; i < 6; i++)
        {
            uret += harfler[rastgele.Next(0, harfler.Length)];
        }

        uret += DateTime.Now.Millisecond;
        return uret;
    }

    public static string clearFileName(this string fileName, string replaceCharacter = "", string guidStr = "")
    {
        var fname = fileName.Replace(@"\", "");
        fname = fileName.Trim('"');
        fname = (fname.StartsWith(".") ? fname.Substring(1, fname.Length - 1) : fname);
        fname = Path.GetFileName(fname.Trim('"'));
        //var ex = Path.GetExtension(fname);
        var ex = fname.Split('.')[1];
        fname = fname.Split('.')[0].Replace(".", "");
        fname = fname.ceoUrl("_");
        fname = (!string.IsNullOrEmpty(guidStr) ? guidStr + replaceCharacter : "") + fname + replaceCharacter + toRandom(fname.Replace("_", "")) + "." + ex;
        return fname;
    }

    public static string ceoUrl(this string url, string replaceCharacter = "")
    {
        if (string.IsNullOrEmpty(url)) return "";
        url = url.ToLower();
        url = url.Trim();
        //if (url.Length > 100)
        //{
        //    url = url.Substring(0, 100);
        //}
        url = url.Replace("İ", "I");
        url = url.Replace("ı", "i");
        url = url.Replace("ğ", "g");
        url = url.Replace("Ğ", "G");
        url = url.Replace("ç", "c");
        url = url.Replace("Ç", "C");
        url = url.Replace("ö", "o");
        url = url.Replace("Ö", "O");
        url = url.Replace("ş", "s");
        url = url.Replace("Ş", "S");
        url = url.Replace("ü", "u");
        url = url.Replace("Ü", "U");
        url = url.Replace("'", "");
        url = url.Replace("\"", "");
        char[] replacerList = @"$%#@!*?;:~`+=()[]{}|\'<>,/^&"".".ToCharArray();
        for (int i = 0; i < replacerList.Length; i++)
        {
            string strChr = replacerList[i].ToString();
            if (url.Contains(strChr))
            {
                url = url.Replace(strChr, replaceCharacter);
            }
        }
        Regex r = new Regex("[^a-zA-Z0-9_-]");
        url = r.Replace(url, replaceCharacter);
        while (url.IndexOf("--") > -1)
            url = url.Replace("--", replaceCharacter);

        return url;
    }

    public static string GetCleanText(this string text)
    {
        string outtext = text.ToLower(System.Globalization.CultureInfo.GetCultureInfo("en"));
        outtext = ClearTurkish(outtext);
        string validchars = "abcdefghijklmnopqrstuvwxyz0123456789";
        string outvalidtext = "";
        for (int x = 0; x < outtext.Length; x++)
            if (validchars.IndexOf(outtext.Substring(x, 1)) != -1)
                outvalidtext += outtext.Substring(x, 1);
            else
                outvalidtext += "-";
        return outvalidtext.Replace("---", "-").Replace("--", "-").Replace("+", "_arti_");
    }
    public static string ClearTurkish(this string input)
    {
        return
            StripHtml(input)
            .Replace("Ç", "C")
            .Replace("ç", "c")
            .Replace("Ğ", "G")
            .Replace("ğ", "g")
            .Replace("İ", "I")
            .Replace("ı", "i")
            .Replace("Ö", "ö")
            .Replace("ö", "o")
            .Replace("Ş", "S")
            .Replace("ş", "s")
            .Replace("Ü", "U")
            .Replace("ü", "u")
            .Replace("â", "a")
            .Replace("?", "-")
            .Replace("=", "-")
            .Replace("!", "-")
            .Replace(" ", "-")
            .Replace("&", "-")
            .Replace("%", "-")
            .Replace("\\", "-")
            .Replace("(", "-")
            .Replace(")", "-")
            .Replace("<", "-")
            .Replace(">", "-")
            .Replace("'", "-")
            .Replace("\"", "-")
            .Replace("\n", "-")
            .Replace(".", "-")
            .Replace(",", "-")
            .Replace("@", "-")
            .Replace(":", "-")
            .Replace(";", "-")
            .Replace("“", "-")
            .Replace("”", "-")
            .Replace("/", "-")
            .Replace("+", "_arti_");
    }

    public static string GetCleanTextSearch(this string text)
    {
        string outtext = text.ToLower(System.Globalization.CultureInfo.GetCultureInfo("en"));
        string validchars = "abcçdefgğhıijklmnoöpqrsştuüvwxyz0123456789 ";
        string outvalidtext = "";
        for (int x = 0; x < outtext.Length; x++)
            if (validchars.IndexOf(outtext.Substring(x, 1)) != -1)
                outvalidtext += text.Substring(x, 1);
            else
                outvalidtext += "";

        string removedless2chars = "";
        foreach (string holder in outvalidtext.Trim().Split(' '))
        {
            if (holder.Trim().Length > 2)
            {
                removedless2chars += holder;
                removedless2chars += " ";
            }
        }
        return removedless2chars.Trim();
    }

    public static string ReplaceIllegalCharacters(this string val, string replaceCharacter = "")
    {
        replaceCharacter = replaceCharacter ?? "";
        val = val.Replace(" ", "")
                    .Replace("!", replaceCharacter)
                    .Replace("'", replaceCharacter)
                    .Replace("^", replaceCharacter)
                    .Replace("%", replaceCharacter)
                    .Replace("&", replaceCharacter)
                    .Replace("/", replaceCharacter)
                    .Replace("(", replaceCharacter)
                    .Replace(")", replaceCharacter)
                    .Replace("=", replaceCharacter)
                    .Replace("?", replaceCharacter)
                    .Replace("<", replaceCharacter)
                    .Replace(">", replaceCharacter)
                    .Replace("£", replaceCharacter)
                    .Replace("#", replaceCharacter)
                    .Replace("½", replaceCharacter)
                    .Replace("{", replaceCharacter)
                    .Replace("[", replaceCharacter)
                    .Replace("]", replaceCharacter)
                    .Replace("}", replaceCharacter)
                    .Replace("\\", replaceCharacter)
                    .Replace("|", replaceCharacter)
                    .Replace("*", replaceCharacter)
                    .Replace("é", replaceCharacter)
                    .Replace("¨", replaceCharacter)
                    .Replace("~", replaceCharacter)
                    .Replace("`", replaceCharacter)
                    .Replace(";", replaceCharacter)
                    .Replace(":", replaceCharacter)
                    .Replace(" ", replaceCharacter);
        return val;
    }


    public static string SetImage(this string ImageUrl, string BaseUrl)
    {
        return !string.IsNullOrEmpty(ImageUrl) ? BaseUrl + "/fileupload/UserFiles/Folders/" + ImageUrl : "";
    }



    public static string GetDisplayName(this PropertyInfo prop)
    {
        if (prop.CustomAttributes == null || prop.CustomAttributes.Count() == 0)
            return prop.Name;

        var displayNameAttribute = prop.CustomAttributes.Where(x => x.AttributeType == typeof(DisplayNameAttribute)).FirstOrDefault();

        if (displayNameAttribute == null || displayNameAttribute.ConstructorArguments == null || displayNameAttribute.ConstructorArguments.Count == 0)
            return prop.Name;

        return displayNameAttribute.ConstructorArguments[0].Value.ToString() ?? prop.Name;
    }

    public static string strSqlColumn(this DataTable table)
    {
        string sql = "";
        foreach (DataColumn column in table.Columns)
        {
            if (sql.Length > 0)
                sql += ", ";
            sql += column.ColumnName;
        }
        return sql;
    }





    public static string toSqlInsertInto<T>(this List<T> _list)
    {
        var baseType = new BaseModel().GetType().GetProperties().Where(o => o.Name != "Id" && o.Name != "CreaDate" && o.Name != "CreaUser").ToList();
        var tablename = typeof(T).Name;
        var str = Environment.NewLine + @"------------START " + tablename + "---------------------\n";

        _list.ForEach(o =>
        {
            str += Environment.NewLine + @"insert into " + @"""" + tablename + @"""" + Environment.NewLine;
            var i = 0;
            var prop = o.GetType().GetProperties().ToList();
            str += " (";
            prop.ForEach(pp =>
            {
                var value = o.GetPropValue(pp.Name);
                if (value == null || value == "" || baseType.Any(cc => cc.Name == pp.Name)) return;
                str += i != 0 ? "," : "";
                i++;
                str += @"""" + pp.Name + @"""";
            });
            str += @" ) " + Environment.NewLine + "  values (";

            i = 0;
            prop.ForEach(pp =>
            {
                var value = o.GetPropValue(pp.Name);
                if (value == null || value == "" || baseType.Any(cc => cc.Name == pp.Name)) return;

                str += i != 0 ? "," : "";
                i++;

                switch (Type.GetTypeCode(pp.PropertyType))
                {
                    case TypeCode.Boolean:
                        {
                            value = value.ToBoolean();
                            break;
                        }
                    case TypeCode.SByte:
                    case TypeCode.Byte:
                    case TypeCode.Int16:
                    case TypeCode.UInt16:
                    case TypeCode.Int32:
                    case TypeCode.UInt32:
                    case TypeCode.Int64:
                    case TypeCode.UInt64:
                    case TypeCode.Single:
                        {
                            value = value.ToInt();
                            break;
                        }
                    case TypeCode.Double:
                        {
                            value = value.ToDouble();
                            break;
                        }
                    case TypeCode.Decimal:
                        {
                            value = value.ToDecimal();
                            break;
                        }
                    case TypeCode.DateTime:
                        {
                            value = @"'" + value.ToStr() + @"'";
                            break;
                        }
                    case TypeCode.String:
                        {
                            value = @"'" + value.ToStr() + @"'";
                            break;
                        }
                    case TypeCode.Object:
                        {
                            //value = value.ToInt();
                            break;
                        }
                    default:
                        {
                            //value = value.ToInt();
                            break;
                        }
                }

                if (pp.Name == "CreaUser")
                    str += 1;
                else if (pp.Name == "CreaDate")
                    str += @"'" + DateTime.Now.ToString() + @"'";
                else
                    str += value;


            });
            str += " );";
        });
        str += Environment.NewLine + Environment.NewLine + @"------------END " + tablename + "---------------------" + Environment.NewLine + Environment.NewLine;
        return str;
    }


    public static Expression<Func<T, bool>> Combine<T>(Expression<Func<T, bool>> expression1, Expression<Func<T, bool>> expression2)
    {
        if (expression1 == null && expression2 == null)
        {
            return null;
        }

        if (expression1 == null)
        {
            return expression2;
        }

        if (expression2 == null)
        {
            return expression1;
        }

        var parameter = Expression.Parameter(typeof(T));

        var leftVisitor = new ReplaceExpressionVisitor(expression1.Parameters[0], parameter);
        var left = leftVisitor.Visit(expression1.Body);

        var rightVisitor = new ReplaceExpressionVisitor(expression2.Parameters[0], parameter);
        var right = rightVisitor.Visit(expression2.Body);

        return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(left, right), parameter);
    }



    public static Dictionary<string, object> GetPropertyAttributes(PropertyInfo property)
    {
        Dictionary<string, object> attribs = new Dictionary<string, object>();
        foreach (CustomAttributeData attribData in property.GetCustomAttributesData())
        {
            string typeName = attribData.Constructor.DeclaringType.Name;
            if (typeName.EndsWith("Attribute"))
                typeName = typeName.Substring(0, typeName.Length - 9);

            if (typeName == "Required")
                attribs[typeName] = typeName.ToLower();
            if (typeName == "NotMapped")
                attribs[typeName] = typeName.ToLower();
            else if (typeName == "DataType")
                attribs[typeName] = attribData.ConstructorArguments[0].Value;
            else
                attribs[typeName] = attribData.ConstructorArguments.Count > 0 ? attribData.ConstructorArguments[0].Value : "";
        }
        return attribs;
    }

    public static void SetValueCustom(this object container, string propertyName, object value)
    {
        container.GetType()?.GetProperty(propertyName)?.SetValue(container, value, null);
    }

    public static object GetPropValue(this object obj, string fieldName)
    { //<-- fieldName = "Details.Name"
        object value = null;
        string[] nameParts = fieldName.Split('.');
        foreach (String part in nameParts)
        {
            if (obj == null) { return ""; }

            Type type = obj.GetType();
            PropertyInfo info = type.GetProperty(part);
            if (info == null)
            {
                var infoField = type.GetField(part);
                if (infoField == null)
                    return "";
                else
                {
                    if (infoField.FieldType.Name.ToLower().Contains("collection") || infoField.FieldType.Name.ToLower().Contains("list"))
                        try
                        {
                            value = (infoField.GetValue(obj) as List<EnumModel>)
                            .GroupBy(o => o.value).Select(o => new EnumModel { value = o.Key, text = o.FirstOrDefault().text, name = o.FirstOrDefault().name }).ToList();
                        }
                        catch (Exception ex)
                        {
                        }

                    if (value == null)
                        value = infoField.GetValue(obj);


                    return value;
                }
            }

            if (info.PropertyType.Name.ToLower().Contains("collection") || info.PropertyType.Name.ToLower().Contains("List"))
            {
                if (fieldName == "LangText")
                    value = info.GetValue(obj, null);
                else
                {
                    try
                    {
                        value = info.GetValue(obj, null) as IList;

                    }
                    catch (Exception ex)
                    {
                    }
                    if (value == null)
                    {
                        try
                        {
                            value = (info.GetValue(obj) as List<EnumModel>)
                                .GroupBy(o => o.value).Select(o => new EnumModel { value = o.Key, text = o.FirstOrDefault().text, name = o.FirstOrDefault().name }).ToList();
                        }
                        catch (Exception ex)
                        {

                        }
                    }


                }

            }
            else
                value = info.GetValue(obj, null);

        }
        return value;
    }


    private static Dictionary<Type, PropertyInfo[]> _TypesWithWriteableProperties = new Dictionary<Type, PropertyInfo[]>();


    public static Expression<Func<T, bool>> filter<T>(IList<Expression<Func<T, bool>>> predicateExpressions,
    IList<Func<Expression, Expression, BinaryExpression>> logicalFunctions)
    {
        Expression<Func<T, bool>> filter = null;

        if (predicateExpressions.Count > 0)
        {
            Expression<Func<T, bool>> firstPredicate = predicateExpressions[0];
            Expression body = firstPredicate.Body;
            for (int i = 1; i < predicateExpressions.Count; i++)
            {
                body = logicalFunctions[i - 1](body, predicateExpressions[i].Body);
            }
            filter = Expression.Lambda<Func<T, bool>>(body, firstPredicate.Parameters);
        }

        return filter;
    }

    //public static IEnumerable<T> filter<T>(IEnumerable<T> source, string columnName, string propertyValue)
    //{
    //    return source.Where(m => { return m.GetType().GetProperty(columnName).GetValue(m, null).ToString().StartsWith(propertyValue); });
    //}

    //public static Func<T, bool> filter<T>(this T model)
    //{

    //    PropertyInfo[] props = t.GetProperties();
    //    foreach (PropertyInfo o in props)
    //    {
    //        var param = Expression.Parameter(typeof(T));


    //        object value = o.GetValue(model, new object[] { });
    //        if (value != null && !o.Name.Contains("Date"))
    //        {

    //            var condition =
    //                             Expression.Lambda<Func<T, bool>>(
    //                                 Expression.Equal(
    //                                     Expression.Property(param, o.Name),
    //                                     Expression.Constant(value, typeof(int))
    //                                 ),
    //                                 param
    //                             ).Compile(); // for LINQ to SQl/Entities skip Compile() call

    //            var body = condition.GetPredicateExpression(param);
    //            var dd = Expression.AndAlso();

    //        }
    //    }



    //    return propList;
    //}


    /// <summary>
    /// Rotate the given image file according to Exif Orientation data
    /// </summary>
    /// <param name="sourceFilePath">path of source file</param>
    /// <param name="targetFilePath">path of target file</param>
    /// <param name="targetFormat">target format</param>
    /// <param name="updateExifData">set it to TRUE to update image Exif data after rotation (default is TRUE)</param>
    /// <returns>The RotateFlipType value corresponding to the applied rotation. If no rotation occurred, RotateFlipType.RotateNoneFlipNone will be returned.</returns>
    public static RotateFlipType RotateImageByExifOrientationData(string sourceFilePath, string targetFilePath, ImageFormat targetFormat, bool updateExifData = true)
    {
        // Rotate the image according to EXIF data
        var bmp = new Bitmap(sourceFilePath);
        RotateFlipType fType = RotateImageByExifOrientationData(bmp, updateExifData);
        if (fType != RotateFlipType.RotateNoneFlipNone)
        {
            bmp.Save(targetFilePath, targetFormat);
        }
        return fType;
    }

    /// <summary>
    /// Rotate the given bitmap according to Exif Orientation data
    /// </summary>
    /// <param name="img">source image</param>
    /// <param name="updateExifData">set it to TRUE to update image Exif data after rotation (default is TRUE)</param>
    /// <returns>The RotateFlipType value corresponding to the applied rotation. If no rotation occurred, RotateFlipType.RotateNoneFlipNone will be returned.</returns>
    public static RotateFlipType RotateImageByExifOrientationData(Image img, bool updateExifData = true)
    {
        int orientationId = 0x0112;
        var fType = RotateFlipType.RotateNoneFlipNone;
        if (img.PropertyIdList.Contains(orientationId))
        {
            var pItem = img.GetPropertyItem(orientationId);
            fType = GetRotateFlipTypeByExifOrientationData(pItem.Value[0]);
            if (fType != RotateFlipType.RotateNoneFlipNone)
            {
                img.RotateFlip(fType);
                // Remove Exif orientation tag (if requested)
                if (updateExifData) img.RemovePropertyItem(orientationId);
            }
        }
        return fType;
    }

    /// <summary>
    /// Return the proper System.Drawing.RotateFlipType according to given orientation EXIF metadata
    /// </summary>
    /// <param name="orientation">Exif "Orientation"</param>
    /// <returns>the corresponding System.Drawing.RotateFlipType enum value</returns>
    public static RotateFlipType GetRotateFlipTypeByExifOrientationData(int orientation)
    {
        switch (orientation)
        {
            case 1:
            default:
                return RotateFlipType.RotateNoneFlipNone;
            case 2:
                return RotateFlipType.RotateNoneFlipX;
            case 3:
                return RotateFlipType.Rotate180FlipNone;
            case 4:
                return RotateFlipType.Rotate180FlipX;
            case 5:
                return RotateFlipType.Rotate90FlipX;
            case 6:
                return RotateFlipType.Rotate90FlipNone;
            case 7:
                return RotateFlipType.Rotate270FlipX;
            case 8:
                return RotateFlipType.Rotate270FlipNone;
        }
    }



    public static List<string> validControl<T>(this T table, string prop, string errorText)
    {
        List<string> list = new List<string>();

        var p = prop.Split(',').ToList();
        var sourceProperties = table.GetType().GetProperties().ToList();

        p.ForEach(o =>
        {
            var item = sourceProperties.FirstOrDefault(i => i.Name == o);
            if (item != null)
            {
                string cont = item.GetValue(table, null).ToStr();
                if (cont == null || string.IsNullOrEmpty(cont) || cont == "-1")
                {
                    list.Add(item.Name + "  : " + errorText);
                }
            }
        });

        return list;
    }


    //public static string deleteImage(string dosya_yolu, string filename, string ImageW, string ImageH)
    //{
    //    try
    //    {
    //        var boyutList = ConfigurationManager.AppSettings["resim-boyut"].Split(',').ToList();
    //        var controlBoyut = boyutList.Any(o => o == ImageW + "_" + ImageH);
    //        if (!controlBoyut)
    //            boyutList.Add(ImageW + "_" + ImageH);

    //        foreach (var boyut in boyutList)
    //        {
    //            string fullpath = dosya_yolu + (boyut + "_" + filename);
    //            var ff = new FileInfo(fullpath);
    //            if (ff.Exists)
    //                ff.Delete();
    //        }
    //        return "true";
    //    }
    //    catch (Exception ex)
    //    {
    //        return ex.Message;
    //    }

    //}

    //public static Bitmap saveImage(HttpPostedFileBase fu, int width, int height, string dosya_yolu, string filename, string fileType, string filigran)
    //{
    //    string[] kes;
    //    var img = new Bitmap(fu.InputStream);

    //    var boyutList = ConfigurationManager.AppSettings["resim-boyut"].Split(',').ToList();
    //    var controlBoyut = boyutList.Any(o => o == img.Width + "_" + img.Height);
    //    if (!controlBoyut)
    //        boyutList.Add(img.Width + "_" + img.Height);
    //    foreach (var boyut in boyutList)
    //    {
    //        kes = boyut.Split('_');
    //        width = Convert.ToInt32(kes[0]);
    //        height = Convert.ToInt32(kes[1]);
    //        string fullpath = (dosya_yolu + (boyut + "_" + filename + "." + fileType)).Trim();
    //        var imgResize = ResizeImage(img, new Size(width, height), filigran);


    //        imgResize.Save(fullpath, img.RawFormat);
    //    }
    //    return img;
    //}


    public static Bitmap ResizeImage(Bitmap imgToResize, Size size, string filigran)
    {
        //Image'in Aspect Ratio Oranını Bul
        double dblRatio = (double)imgToResize.Width / (double)imgToResize.Height;

        //1-) Gelen Width ve Height Değerlerinden "0" Olan Var ise, buna karşılık gelen değer, Image'in Aspect Ratio Oranına Göre Bulunur.
        size.Width = size.Width == 0 ? (int)(size.Height * dblRatio) : size.Width;
        size.Height = size.Height == 0 ? (int)(size.Width / dblRatio) : size.Height;

        //2-) Eğer olması istenen boyutlardan biri, image'in gerçek boyutundan büyük ise, image'in orjinal boyutuna göre işleme devam edilir.
        size.Width = size.Width > imgToResize.Width ? imgToResize.Width : size.Width;
        size.Height = size.Height > imgToResize.Height ? imgToResize.Height : size.Height;

        //3-) Eğer belirtilen yeni boyutlar image'in yeni aspect ratio'suna uymuyor ise, büyük olan boyut sabit alınarak, diğeri aspect ratio oranına göre bulunur.
        double dblResizeRatio = (double)size.Width / (double)size.Height;
        if (Math.Abs(dblResizeRatio - dblRatio) > 0.01)
        {
            //Büyük olan oranı bul ve diğerini Aspect ratio oranını koruyarak değiştirilir.
            if (size.Width > size.Height)
            {
                size.Height = (int)(size.Width / dblRatio);
            }
            else
            {
                size.Width = (int)(size.Height * dblRatio);
            }
        }

        Bitmap imgResize = new Bitmap(size.Width, size.Height);
        using (Graphics g = Graphics.FromImage((Image)imgResize))
        {
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.DrawImage(imgToResize, 0, 0, size.Width, size.Height);

            Bitmap watermark = new Bitmap(filigran);
            watermark = ResizeImageFiligran(watermark, new Size() { Width = imgResize.Width, Height = imgResize.Height }, filigran);

            var destX = (imgResize.Width - watermark.Width) / 2;
            var destY = (imgResize.Height - watermark.Height) / 2;

            g.DrawImage(watermark, new Rectangle(destX,
                        destY,
                        watermark.Width,
                        watermark.Height));

            //using (TextureBrush br = new TextureBrush(imgToResize,
            //new Rectangle(0, 0, imgToResize.Width - 1, imgToResize.Height - 1)))
            //{
            //    g.FillRectangle(br, 0, 0, watermark.Width, watermark.Height);
            //}

            // display a clone for demo purposes
        }

        //Bitmap filigranImg = new Bitmap(filigran);
        //var lastImg = WatermarkImage(imgResize, filigranImg);
        return imgResize;

    }

    public static Bitmap ResizeImageFiligran(Bitmap imgToResize, Size size, string filigran)
    {
        //Image'in Aspect Ratio Oranını Bul
        double dblRatio = (double)imgToResize.Width / (double)imgToResize.Height;

        //1-) Gelen Width ve Height Değerlerinden "0" Olan Var ise, buna karşılık gelen değer, Image'in Aspect Ratio Oranına Göre Bulunur.
        size.Width = size.Width == 0 ? (int)(size.Height * dblRatio) : size.Width;
        size.Height = size.Height == 0 ? (int)(size.Width / dblRatio) : size.Height;

        //2-) Eğer olması istenen boyutlardan biri, image'in gerçek boyutundan büyük ise, image'in orjinal boyutuna göre işleme devam edilir.
        //size.Width = size.Width > imgToResize.Width ? imgToResize.Width : size.Width;
        //size.Height = size.Height > imgToResize.Height ? imgToResize.Height : size.Height;

        //3-) Eğer belirtilen yeni boyutlar image'in yeni aspect ratio'suna uymuyor ise, büyük olan boyut sabit alınarak, diğeri aspect ratio oranına göre bulunur.
        double dblResizeRatio = (double)size.Width / (double)size.Height;
        if (Math.Abs(dblResizeRatio - dblRatio) > 0.01)
        {
            //Büyük olan oranı bul ve diğerini Aspect ratio oranını koruyarak değiştirilir.
            if (size.Width > size.Height)
            {
                size.Height = (int)(size.Width / dblRatio);
            }
            else
            {
                size.Width = (int)(size.Height * dblRatio);
            }
        }

        Bitmap imgResize = new Bitmap(size.Width, size.Height);
        using (Graphics g = Graphics.FromImage((Image)imgResize))
        {
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.DrawImage(imgToResize, 0, 0, size.Width, size.Height);

        }
        return imgResize;

    }


    public static Bitmap WatermarkImage(Bitmap image, Bitmap watermark)
    {

        using (Graphics imageGraphics = Graphics.FromImage(image))
        {
            watermark.SetResolution(imageGraphics.DpiX, imageGraphics.DpiY);

            int x = (image.Width - watermark.Width) / 2;
            int y = (image.Height - watermark.Height) / 2;

            imageGraphics.DrawImage(watermark, x, y, watermark.Width, watermark.Height);
        }

        //if (!string.IsNullOrEmpty(filigranImagePath))
        //{
        //    using (Image fImage = Image.FromFile(filigranImagePath))
        //    using (Brush watermarkBrush = new TextureBrush(fImage))
        //    {
        //        imgGraph.FillRectangle(watermarkBrush, new Rectangle(new Point(0, 0), fImage.Size));
        //    }
        //}


        return image;
    }


    //public static string saveFiles2(HttpPostedFileBase fu, int genislik, int yukseklik, string dosya_yolu, string filename, string fileType, string filigran)
    //{
    //    try
    //    {
    //        string[] kes;

    //        foreach (var boyut in ConfigurationManager.AppSettings["resim-boyut"].Split(','))
    //        {
    //            //if (Directory.Exists(HttpContext.Current.Server.MapPath(dosya_yolu + boyut)) == false)
    //            //    Directory.CreateDirectory(HttpContext.Current.Server.MapPath(dosya_yolu + boyut));
    //            kes = boyut.Split('_');
    //            genislik = Convert.ToInt32(kes[0]);
    //            yukseklik = Convert.ToInt32(kes[1]);

    //            string fullpath = dosya_yolu + ("\\" + boyut + "_" + filename + "." + fileType);

    //            System.Drawing.Image orjinalFoto = null;
    //            orjinalFoto = System.Drawing.Image.FromStream(fu.InputStream);

    //            var w = orjinalFoto.Width;
    //            var h = orjinalFoto.Height;
    //            if (genislik != 0)
    //            {
    //                w = genislik;
    //            }
    //            if (yukseklik != 0)
    //            {
    //                h = yukseklik;
    //            }

    //            Bitmap bitmap = orjinalFoto as Bitmap;



    //            boyutlandir2(w, h, bitmap, fullpath, filigran);
    //        }



    //        return "true";
    //    }
    //    catch (Exception ex)
    //    {
    //        return ex.Message;
    //    }
    //}

    public static Bitmap Resize2(this string fullPath, string filigranText, string filigranImagePath)
    {
        var image = Image.FromFile(fullPath);

        double oran = 0;
        double genislik = image.Width;
        double yukseklik = image.Height;
        if (genislik < yukseklik)
        {
            oran = genislik / yukseklik;
            genislik = image.Width;
            yukseklik = image.Width / oran;
        }
        else if (genislik > yukseklik)
        {
            oran = yukseklik / genislik;
            genislik = image.Height / oran;
            yukseklik = image.Height;
            if (genislik < image.Width)
            {
                oran = genislik / yukseklik;
                genislik = image.Width;
                yukseklik = image.Width / oran;
            }
        }


        System.Drawing.Size yeniboyut = new System.Drawing.Size(Convert.ToInt32(genislik), Convert.ToInt32(yukseklik));
        int x = (image.Width - image.Width) / 2;
        int y = (image.Height - image.Height) / 2;
        Bitmap bmp = new Bitmap(image.Width, image.Height, image.PixelFormat);
        Graphics grf = Graphics.FromImage(bmp);
        grf.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
        grf.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
        grf.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
        grf.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
        grf.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height), new Rectangle(x, y, image.Width, image.Height), GraphicsUnit.Pixel);
        if (!string.IsNullOrEmpty(filigranText))
        {
            StringFormat Format = new StringFormat();
            Format.Alignment = StringAlignment.Near;
            Format.FormatFlags = StringFormatFlags.NoWrap;
            grf.DrawString(filigranText, new Font("Verdana", image.Width / 25, FontStyle.Bold), new SolidBrush(Color.FromArgb(80, 255, 255, 255)), image.Width / 2, image.Height / 2, Format);
        }

        if (!string.IsNullOrEmpty(filigranImagePath))
        {
            using (Image fImage = Image.FromFile(filigranImagePath))
            using (Brush watermarkBrush = new TextureBrush(fImage))
            {
                grf.FillRectangle(watermarkBrush, new Rectangle(new Point(0, 0), fImage.Size));
            }
        }



        image.Dispose();
        //bmp.Dispose();
        return bmp;
    }

    //base
    public static Image Resize(this Stream sourcePath, string targetPath, string filigranImagePath, string filigranText)
    {

        using (var image = new Bitmap(sourcePath))
        {
            float maxWidth = 1200;
            float maxHeight = 1200;

            int newWidth;
            int newHeight;
            string extension;
            Bitmap originalBMP = new Bitmap(sourcePath);
            int originalWidth = originalBMP.Width;
            int originalHeight = originalBMP.Height;

            if (originalWidth > maxWidth || originalHeight > maxHeight)
            {
                // To preserve the aspect ratio  
                float ratioX = (float)maxWidth / (float)originalWidth;
                float ratioY = (float)maxHeight / (float)originalHeight;
                float ratio = Math.Min(ratioX, ratioY);
                newWidth = (int)(originalWidth * ratio);
                newHeight = (int)(originalHeight * ratio);


            }
            else
            {
                newWidth = (int)originalWidth;
                newHeight = (int)originalHeight;

            }
            Bitmap bitMAP1 = new Bitmap(originalBMP, newWidth, newHeight);
            if (newWidth > newHeight)
            {
                bitMAP1.RotateFlip(RotateFlipType.Rotate90FlipX);
                var temp = newWidth;
                newWidth = newHeight;
                newHeight = temp;
                originalBMP = bitMAP1;
            }
            Graphics imgGraph = Graphics.FromImage(bitMAP1);
            //imgGraph.RotateTransform(-90);
            extension = Path.GetExtension(targetPath);

            var encoder = System.Drawing.Imaging.Encoder.Quality;
            EncoderParameters myEncoderParameters = new EncoderParameters(1);
            EncoderParameter myEncoderParameter = new EncoderParameter(encoder, 72L);
            myEncoderParameters.Param[0] = myEncoderParameter;


            //if (extension.ExToLower() == ".png" || extension.ExToLower() == ".gif")
            //{
            //    ImageCodecInfo typeEncoder = GetEncoderInfo(ImageFormat.Jpeg);
            //    imgGraph.SmoothingMode = SmoothingMode.AntiAlias;
            //    imgGraph.InterpolationMode = InterpolationMode.HighQualityBicubic;
            //    imgGraph.DrawImage(originalBMP, 0, 0, newWidth, newHeight);
            //    //bitMAP1.SetResolution();

            //    if (!string.IsNullOrEmpty(filigranText))
            //    {
            //        StringFormat Format = new StringFormat();
            //        Format.Alignment = StringAlignment.Near;
            //        Format.FormatFlags = StringFormatFlags.NoWrap;
            //        imgGraph.DrawString(filigranText, new Font("Verdana", image.Width / 25, FontStyle.Bold), new SolidBrush(Color.FromArgb(80, 255, 255, 255)), image.Width / 2, image.Height / 2, Format);
            //    }


            //    if (!string.IsNullOrEmpty(filigranImagePath))
            //    {
            //        using (Image fImage = Image.FromFile(targetPath))
            //        {
            //            Rectangle myIconDrawingRectangle = new Rectangle(0, 50, fImage.Width, fImage.Height);
            //            using (TextureBrush brush = new TextureBrush(fImage, WrapMode.Tile))
            //            {
            //                brush.TranslateTransform(0, 50);
            //                imgGraph.FillRectangle(brush, myIconDrawingRectangle);
            //                targetPath = targetPath.toFiligran();
            //                //bitMAP1.Save(targetPath, image.RawFormat);
            //                bitMAP1.Save(targetPath.Replace(".png", ".jpg").Replace(".gif", ".jpg"), typeEncoder, myEncoderParameters);
            //            }
            //        }
            //    }
            //    else
            //    {
            //        bitMAP1.Save(targetPath.Replace(".png", ".jpg").Replace(".gif", ".jpg"), typeEncoder, myEncoderParameters);
            //    }




            //}
            if (extension.ExToLower() == ".jpg" || extension.ExToLower() == ".jpeg")
            {
                ImageCodecInfo typeEncoder = GetEncoderInfo(ImageFormat.Jpeg);
                imgGraph.SmoothingMode = SmoothingMode.HighQuality;
                imgGraph.InterpolationMode = InterpolationMode.HighQualityBicubic;
                imgGraph.CompositingQuality = CompositingQuality.HighSpeed;
                imgGraph.PixelOffsetMode = PixelOffsetMode.HighQuality;
                imgGraph.DrawImage(originalBMP, 0, 0, newWidth, newHeight);


                if (!string.IsNullOrEmpty(filigranText))
                {
                    StringFormat Format = new StringFormat();
                    Format.Alignment = StringAlignment.Near;
                    Format.FormatFlags = StringFormatFlags.NoWrap;
                    imgGraph.DrawString(filigranText, new Font("Verdana", image.Width / 25, FontStyle.Bold), new SolidBrush(Color.FromArgb(80, 255, 255, 255)), image.Width / 2, image.Height / 2, Format);
                }


                if (!string.IsNullOrEmpty(filigranImagePath))
                {
                    using (Image fImage = Image.FromFile(targetPath))
                    {
                        Rectangle myIconDrawingRectangle = new Rectangle(0, 50, fImage.Width, fImage.Height);
                        using (TextureBrush brush = new TextureBrush(fImage, WrapMode.Tile))
                        {
                            brush.TranslateTransform(0, 50);
                            imgGraph.FillRectangle(brush, myIconDrawingRectangle);
                            targetPath = targetPath.toFiligran();
                            bitMAP1.Save(targetPath.Replace(".jpeg", ".jpg"), typeEncoder, myEncoderParameters);
                        }
                    }

                }
                else
                {
                    bitMAP1.Save(targetPath.Replace(".jpeg", ".jpg"), typeEncoder, myEncoderParameters);
                }

            }

            bitMAP1.Dispose();
            imgGraph.Dispose();
            originalBMP.Dispose();

            return image;
        }

    }


    /// <summary>
    /// Method to resize, convert and save the image.
    /// </summary>
    /// <param name="image">Bitmap image.</param>
    /// <param name="maxWidth">resize width.</param>
    /// <param name="maxHeight">resize height.</param>
    /// <param name="quality">quality setting value.</param>
    /// <param name="filePath">file path.</param>      
    public static Image Resize(this string filePath, int maxWidth, int maxHeight, int quality, string filigranText, string filigranImagePath)
    {
        var image = Image.FromFile(filePath);

        // Get the image's original width and height
        int originalWidth = image.Width;
        int originalHeight = image.Height;

        // To preserve the aspect ratio
        float ratioX = (float)maxWidth / (float)originalWidth;
        float ratioY = (float)maxHeight / (float)originalHeight;
        float ratio = Math.Min(ratioX, ratioY);

        // New width and height based on aspect ratio
        int newWidth = (int)(originalWidth * ratio);
        int newHeight = (int)(originalHeight * ratio);

        // Convert other formats (including CMYK) to RGB.
        Bitmap newImage = new Bitmap(newWidth, newHeight, PixelFormat.Format24bppRgb);

        // Draws the image in the specified size with quality mode set to HighQuality
        using (Graphics graphics = Graphics.FromImage(newImage))
        {
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.DrawImage(image, 0, 0, newWidth, newHeight);
        }

        // Get an ImageCodecInfo object that represents the JPEG codec.
        ImageCodecInfo imageCodecInfo = GetEncoderInfo(ImageFormat.Jpeg);

        // Create an Encoder object for the Quality parameter.
        var encoder = System.Drawing.Imaging.Encoder.Quality;

        // Create an EncoderParameters object. 
        EncoderParameters encoderParameters = new EncoderParameters(1);

        // Save the image as a JPEG file with quality level.
        EncoderParameter encoderParameter = new EncoderParameter(encoder, quality);
        encoderParameters.Param[0] = encoderParameter;

        var ff = new FileInfo(filePath);
        image.Dispose();
        if (ff.Exists)
            ff.Delete();

        newImage.Save(filePath, imageCodecInfo, encoderParameters);
        var res = Image.FromFile(filePath);
        return res;
    }

    /// <summary>
    /// Method to get encoder infor for given image format.
    /// </summary>
    /// <param name="format">Image format</param>
    /// <returns>image codec info.</returns>
    private static ImageCodecInfo GetEncoderInfo(ImageFormat format)
    {
        return ImageCodecInfo.GetImageDecoders().SingleOrDefault(c => c.FormatID == format.Guid);
    }

    public static Image Resize1(this string fileName, int newQuality, string filigranText, string filigranImagePath)   // set quality to 1-100, eg 50
    {

        using (Image image = Image.FromFile(fileName))
        using (Image memImage = new Bitmap(image, image.Width, image.Height))
        {

            ImageCodecInfo myImageCodecInfo;
            System.Drawing.Imaging.Encoder myEncoder;
            EncoderParameter myEncoderParameter;
            EncoderParameters myEncoderParameters;
            myImageCodecInfo = GetEncoderInfo("image/jpeg");
            myEncoder = System.Drawing.Imaging.Encoder.Quality;
            myEncoderParameters = new EncoderParameters(1);
            myEncoderParameter = new EncoderParameter(myEncoder, newQuality);
            myEncoderParameters.Param[0] = myEncoderParameter;

            MemoryStream memStream = new MemoryStream();
            memImage.Save(memStream, myImageCodecInfo, myEncoderParameters);
            Image newImage = Image.FromStream(memStream);
            ImageAttributes imageAttributes = new ImageAttributes();
            using (Graphics g = Graphics.FromImage(newImage))
            {
                g.InterpolationMode =
                  System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;  //**
                g.DrawImage(newImage, new Rectangle(Point.Empty, newImage.Size), 0, 0,
                  newImage.Width, newImage.Height, GraphicsUnit.Pixel, imageAttributes);

                if (!string.IsNullOrEmpty(filigranText))
                {
                    StringFormat Format = new StringFormat();
                    Format.Alignment = StringAlignment.Near;
                    Format.FormatFlags = StringFormatFlags.NoWrap;
                    g.DrawString(filigranText, new Font("Verdana", newImage.Width / 25, FontStyle.Bold), new SolidBrush(Color.FromArgb(80, 255, 255, 255)), newImage.Width / 2, newImage.Height / 2, Format);
                }

                if (!string.IsNullOrEmpty(filigranImagePath))
                {
                    using (Image fImage = Image.FromFile(filigranImagePath))
                    using (Brush watermarkBrush = new TextureBrush(fImage))
                    {
                        g.FillRectangle(watermarkBrush, new Rectangle(new Point(0, 0), fImage.Size));
                    }
                }
            }
            return newImage;
        }
    }

    private static ImageCodecInfo GetEncoderInfo(String mimeType)
    {
        ImageCodecInfo[] encoders;
        encoders = ImageCodecInfo.GetImageEncoders();
        foreach (ImageCodecInfo ici in encoders)
            if (ici.MimeType == mimeType) return ici;

        return null;
    }


    public static string ExQuote(this string value)
    {
        return "'" + value.ToStr() + "'";
    }

    public static int GetNumberDigits(int value)
    {
        if (value < 10) return 10;
        int deger = 1;
        int basamak = 1;
        do
        {
            value /= 10;
            basamak++;

        } while (value > 10);

        for (int i = 0; i < basamak; i++)
        {
            deger *= 10;
        }

        return deger;
    }

    public static string Base64Encode(this string plainText)
    {
        if (plainText == null)
            return plainText;

        var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
        return System.Convert.ToBase64String(plainTextBytes);
    }

    public static string Base64Decode(this string base64EncodedData)
    {
        if (base64EncodedData == null)
            return base64EncodedData;

        var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
        return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
    }

    public static DataTable ToDataTable<T>(this List<T> data)
    {
        PropertyDescriptorCollection properties =
            TypeDescriptor.GetProperties(typeof(T));
        DataTable table = new DataTable();
        foreach (PropertyDescriptor prop in properties)
            table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
        foreach (T item in data)
        {
            DataRow row = table.NewRow();
            foreach (PropertyDescriptor prop in properties)
                row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
            table.Rows.Add(row);
        }
        return table;
    }

    public static DataTable ToDataTable<T>(this IList<T> data)
    {
        PropertyDescriptorCollection properties =
            TypeDescriptor.GetProperties(typeof(T));
        DataTable table = new DataTable();
        foreach (PropertyDescriptor prop in properties)
            table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
        foreach (T item in data)
        {
            DataRow row = table.NewRow();
            foreach (PropertyDescriptor prop in properties)
                row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
            table.Rows.Add(row);
        }
        return table;
    }

    public static bool tccontrol1(this string kimlikno)
    {
        kimlikno = kimlikno.Trim();
        if (kimlikno.Length != 11)
        {
            return false;
        }
        int[] sayilar = new int[11];
        for (int i = 0; i < kimlikno.Length; i++)
        {
            sayilar[i] = Int32.Parse(kimlikno[i].ToString());
        }
        int toplam = 0;
        for (int i = 0; i < kimlikno.Length - 1; i++)
        {
            toplam += sayilar[i];
        }
        if (toplam.ToString()[1].ToString() == sayilar[10].ToString() & sayilar[10] % 2 == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static bool tccontrol2(this string kimlikno)
    {
        bool returnvalue = false;
        if (kimlikno.Length == 11)
        {
            Int64 ATCNO, BTCNO, TcNo;
            long C1, C2, C3, C4, C5, C6, C7, C8, C9, Q1, Q2;

            TcNo = Int64.Parse(kimlikno);

            ATCNO = TcNo / 100;
            BTCNO = TcNo / 100;

            C1 = ATCNO % 10; ATCNO = ATCNO / 10;
            C2 = ATCNO % 10; ATCNO = ATCNO / 10;
            C3 = ATCNO % 10; ATCNO = ATCNO / 10;
            C4 = ATCNO % 10; ATCNO = ATCNO / 10;
            C5 = ATCNO % 10; ATCNO = ATCNO / 10;
            C6 = ATCNO % 10; ATCNO = ATCNO / 10;
            C7 = ATCNO % 10; ATCNO = ATCNO / 10;
            C8 = ATCNO % 10; ATCNO = ATCNO / 10;
            C9 = ATCNO % 10; ATCNO = ATCNO / 10;
            Q1 = ((10 - ((((C1 + C3 + C5 + C7 + C9) * 3) + (C2 + C4 + C6 + C8)) % 10)) % 10);
            Q2 = ((10 - (((((C2 + C4 + C6 + C8) + Q1) * 3) + (C1 + C3 + C5 + C7 + C9)) % 10)) % 10);

            returnvalue = ((BTCNO * 100) + (Q1 * 10) + Q2 == TcNo);
        }
        return returnvalue;
    }


    /// <summary>
    /// Add Quote to string value
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string ExToUpper(this string value)
    {
        string deger = string.Empty;
        for (int i = 0; i < value.Length; i++)
        {
            deger += GetUpperChar(value[i]);
        }
        return deger.ToUpper();

    }

    public static string ExToLower(this string value)
    {
        string deger = string.Empty;
        for (int i = 0; i < value.Length; i++)
        {
            deger += GetLowerChar(value[i]);
        }
        return deger;
    }

    private static string GetUpperChar(char value)
    {
        string deger = value.ToString();
        switch (value)
        {
            case 'i': deger = "I"; break;
            default:
                break;
        }
        return deger;
    }

    private static string GetLowerChar(char value)
    {
        string deger = value.ToString();
        switch (value)
        {
            case 'I': deger = "i"; break;
            default:
                break;
        }

        return deger.ToLower();
    }

    public static Stream ToStream(this string str)
    {
        MemoryStream stream = new MemoryStream();
        StreamWriter writer = new StreamWriter(stream);
        writer.Write(str);
        writer.Flush();
        stream.Position = 0;
        return stream;
    }
    public static string ToStr(this object key)
    {
        return key == null ? string.Empty : key.ToString();
    }

    public static char ToChar(this object key)
    {
        return Convert.ToChar(key);
    }

    public static string ToStrDate(this DateTime key, string format)
    {
        return key == null || key.Year < 1900 ? "" : key.ToString("dd" + format + "MM" + format + "yyyy");
    }

    public static bool ToBoolean(this object key)
    {
        bool deger = false;
        if (key != null)
        {
            if (key.ToString().Contains("True") || key.ToString().Contains("False"))
            {
                bool.TryParse(key.ToString(), out deger);
            }
            else
            {
                deger = Convert.ToBoolean(key.ToInt());
            }
        }
        return deger;
    }

    public static int GetMonthsBetween(this DateTime from, DateTime to)
    {
        if (from > to) return GetMonthsBetween(to, from);

        var monthDiff = Math.Abs((to.Year * 12 + (to.Month - 1)) - (from.Year * 12 + (from.Month - 1)));

        if (from.AddMonths(monthDiff) > to || to.Day < from.Day)
        {
            return monthDiff - 1;
        }
        else
        {
            return monthDiff;
        }
    }

    public static bool IsNullOrEmpty(this string text)
    {
        return !string.IsNullOrEmpty(text) ? false : true;
    }

    public static int ToInt(this object key)
    {
        int value = 0;
        if (key != null)
            int.TryParse(key.ToString(), out value);
        return value;
    }

    public static long ToLong(this object key)
    {
        long value = 0;
        if (key != null)
            long.TryParse(key.ToString(), out value);
        return value;
    }

    public static int ToInt(this object key, int value)
    {
        int ret = value;
        if (key != null)
        {
            if (!int.TryParse(key.ToString(), out value))
                value = ret;
        }
        return value;
    }

    public static decimal ToDecimal(this object key, CultureInfo Culture)
    {
        decimal value = 0;
        if (key != null)
            decimal.TryParse(key.ToString(), NumberStyles.Any, Culture, out value);
        return value;
    }

    public static decimal ToDecimal(this object key)
    {
        decimal value = 0;
        if (key != null)
            decimal.TryParse(key.ToString(), NumberStyles.Any, new CultureInfo("tr-TR"), out value);
        return value;
    }

    public static decimal ToDecimal(this object key, decimal value, CultureInfo Culture)
    {
        decimal ret = value;
        if (key != null)
        {
            if (!decimal.TryParse(key.ToString(), NumberStyles.Any, Culture, out value))
                value = ret;
        }
        return value;
    }

    public static decimal ToDecimal(this object key, decimal value)
    {
        decimal ret = value;
        if (key != null)
        {
            if (!decimal.TryParse(key.ToString(), NumberStyles.Any, new CultureInfo("tr-TR"), out value))
                value = ret;
        }
        return value;
    }
    public static string toFixed(this double number, uint decimals)
    {
        return number.ToString("N" + decimals);
    }

    public static double ToDouble(this object key)
    {
        double value = 0;
        if (key != null)
            double.TryParse(key.ToString(), NumberStyles.Any, new CultureInfo("tr-TR"), out value);
        return value;
    }

    public static double ToDouble(this object key, CultureInfo Culture)
    {
        double value = 0;
        if (key != null)
            double.TryParse(key.ToString(), NumberStyles.Any, Culture, out value);
        return value;
    }

    public static double ToDouble(this object key, double value, CultureInfo Culture)
    {
        double ret = value;
        if (key != null)
        {
            if (!double.TryParse(key.ToString(), NumberStyles.Any, Culture, out value))
                value = ret;
        }
        return value;
    }

    public static DateTime? ToDateTime(this object key)
    {
        DateTime? deger = null;
        if (key != null)
        {
            try
            {
                var date = new DateTime();
                if (DateTime.TryParse(key.ToString(), out date))
                    return date;
                else
                    return null;
            }
            catch
            {
                Console.WriteLine(@"AIop81F9y0ORvI5v98QT");
            }
        }
        return deger;
    }
    public static string ToYMD(this DateTime theDate)
    {
        return theDate.ToString("yyyyMMdd");
    }

    public static string ToYMD(this DateTime? theDate)
    {
        return theDate.HasValue ? theDate.Value.ToYMD() : string.Empty;
    }

    public static int TryParseToInt(this string Deger, int value)
    {
        int.TryParse(Deger, out value);
        return value;
    }

    public static double TryParseToDouble(this string Deger, double value)
    {
        double.TryParse(Deger, out value);
        return value;
    }

    public static decimal TryParseToDecimal(this string deger, decimal value)
    {
        decimal.TryParse(deger, out value);
        return decimal.Round(value, 2);
    }

    public static decimal TryParseToDecimal(this string deger, decimal value, bool round)
    {
        decimal.TryParse(deger, out value);
        if (round) return decimal.Round(value, 2);
        else return value;
    }

    public static string QuotedStr(this object columnValue)
    {
        if (columnValue == null) columnValue = "";
        switch (Type.GetTypeCode(columnValue.GetType()))
        {
            case TypeCode.String:
                return "'" + EscapeText(columnValue.ToString()) + "'";
            default:
                return EscapeText(columnValue.ToString());
        }
    }

    public static string EscapeText(string textToEscape)
    {
        string backslashesEscaped = textToEscape.Replace(@"\", @"\\");
        string backslashAndSingleQuoteEscaped = backslashesEscaped.Replace(@"'", @"\'");
        return backslashAndSingleQuoteEscaped;
    }

    public static int ExGetWeekIndex(this DayOfWeek week)
    {
        int value = -1;

        switch (week)
        {
            case DayOfWeek.Friday:
                value = 4;
                break;
            case DayOfWeek.Monday:
                value = 0;
                break;
            case DayOfWeek.Saturday:
                value = 5;
                break;
            case DayOfWeek.Sunday:
                value = 6;
                break;
            case DayOfWeek.Thursday:
                value = 3;
                break;
            case DayOfWeek.Tuesday:
                value = 1;
                break;
            case DayOfWeek.Wednesday:
                value = 2;
                break;
            default:
                break;
        }

        return value;
    }

    public static bool IsDate(String str)
    {
        bool res = false;
        try
        {
            System.DateTime dt = System.DateTime.Parse(str);
            res = true;
        }
        catch
        {
            Console.WriteLine(@"rKrZpQ7cKEdfARKM7swZ");
            // Not a date, handle appropriately
            res = false;
        }
        return res;
    }

    public static bool IsBetween<T>(this T value, T min, T max) where T : IComparable
    {
        //return Comparer<T>.Default.Compare(value, min) >= 0
        //    && Comparer<T>.Default.Compare(value, max) <= 0;
        return (min.CompareTo(value) <= 0) && (value.CompareTo(max) <= 0);
    }

    public static bool InRange<T>(this T value, params T[] values)
    {
        // Should be even number of items
        Debug.Assert(values.Length % 2 == 0);

        for (int i = 0; i < values.Length; i += 2)
            if (!value.InRange(values[i], values[i + 1]))
                return false;

        return true;
    }

    public static Task<List<T>> ToListAsync<T>(this IQueryable<T> list)
    {
        return Task.Run(() => list.ToList());
    }

    /// <summary>
    /// Verilen doğum tarihine göre yaş bilgisi döner
    /// </summary>
    /// <param name="BirthDate"></param>
    /// <returns></returns>
    public static int ExGetAge(this DateTime? BirthDate)
    {
        if (BirthDate.HasValue)
            return DateTime.Now.Date.Subtract(BirthDate.Value.Date).TotalDays.ToInt() / 365;
        else
            return 0;
    }

    public static int ExGetAge(this DateTime BirthDate)
    {
        return DateTime.Now.Date.Subtract(BirthDate.Date).TotalDays.ToInt() / 365;
    }

    public static int ExGetAge(this DateTime BirthDate, DateTime RegisterDate)
    {
        return RegisterDate.Date.Subtract(BirthDate.Date).TotalDays.ToInt() / 365;
    }

    #region WEBSERVICE

    public static void RemoveTimezoneForDataSet(this DataSet ds)
    {
        foreach (DataTable dt in ds.Tables)
        {
            foreach (DataColumn dc in dt.Columns)
            {

                if (dc.DataType == typeof(DateTime))
                {
                    dc.DateTimeMode = DataSetDateTime.Unspecified;
                }
            }
        }
    }

    #endregion

    #region ENUMS

    public static List<KeyValuePair<string, int>> GetEnumList<T>()
    {
        var list = new List<KeyValuePair<string, int>>();
        foreach (var e in Enum.GetValues(typeof(T)))
        {
            list.Add(new KeyValuePair<string, int>(e.ToString(), (int)e));
        }
        return list;
    }

    //public static List<EnumModel> GetEnumList<T>()
    //{
    //    var list = Enum.GetValues(typeof(T)).Cast<int>().Select(x => new EnumModel { name = x.ToStr(), value = x.ToString(), text = (typeof(T)).ExGetDescription() }).ToList();
    //    return list;
    //}




    public static string ExGetEnumDescription(this string Text, Enum value)
    {
        if (value == null)
            Text = string.Empty;

        FieldInfo fi = value.GetType().GetField(value.ToString());

        if (fi == null)
            Text = string.Empty;

        DescriptionAttribute[] attributes =
            (DescriptionAttribute[])fi.GetCustomAttributes(
            typeof(DescriptionAttribute),
            false);

        if (attributes != null &&
            attributes.Length > 0)
            Text = attributes[0].Description;
        else
            Text = value.ToStr();

        return Text;
    }

    public static string ExGetDescription(this Enum value)
    {
        if (value == null)
            return string.Empty;

        FieldInfo fi = value.GetType().GetField(value.ToString());

        if (fi == null)
            return string.Empty;

        DescriptionAttribute[] attributes =
            (DescriptionAttribute[])fi.GetCustomAttributes(
            typeof(DescriptionAttribute),
            false);

        if (attributes != null &&
            attributes.Length > 0)
            return attributes[0].Description;
        else
            return value.ToStr();
    }

    public static string ExGetDescription(this PropertyInfo property)
    {
        if (property == null)
            return string.Empty;

        DescriptionAttribute[] attributes =
            (DescriptionAttribute[])property.GetCustomAttributes(typeof(DescriptionAttribute), false);

        if (attributes != null &&
            attributes.Length > 0)
            return attributes[0].Description;
        else
            return property.Name.ToStr();
    }

    public static string ExGetDescription(this Type property)
    {
        if (property == null)
            return string.Empty;

        DescriptionAttribute[] attributes =
            (DescriptionAttribute[])property.GetCustomAttributes(typeof(DescriptionAttribute), false);

        if (attributes != null &&
            attributes.Length > 0)
            return attributes[0].Description;
        else
            return property.Name.ToStr();
    }
    #endregion

    #region Data Extension

    public static T NewClass<T>(this T source) where T : class, new()
    {
        var destination = new T();
        PropertyInfo[] properties;
        Type type = typeof(T);
        lock (_TypesWithWriteableProperties)
        {
            if (!_TypesWithWriteableProperties.TryGetValue(type, out properties))
            {
                List<PropertyInfo> props = new List<PropertyInfo>();
                PropertyInfo[] classProps = type.GetProperties();
                foreach (var prop in classProps)
                {
                    if (prop.CanWrite)
                    {
                        props.Add(prop);
                    }
                }
                properties = props.ToArray();
                _TypesWithWriteableProperties[type] = properties;
            }
        }

        foreach (var prop in properties)
        {
            object value = prop.GetValue(source);
            try
            {
                prop.SetValue(destination, value);
            }
            catch
            {
                Console.WriteLine(@"kNWvMLUwguPX9gSnB70n");
            }
        }
        return destination;
    }

    public static T Copy<T, U>(this U source) where T : class, new()
    {
        var destination = new T();
        var destinationProperties = destination.GetType().GetProperties().ToList();
        var sourceProperties = source.GetType().GetProperties().ToList();

        foreach (var destinationProperty in destinationProperties)
        {
            var sourceProperty = sourceProperties.Find(item => item.Name == destinationProperty.Name);

            if (sourceProperty != null && destinationProperty.CanWrite)
            {
                try
                {
                    destinationProperty.SetValue(destination, sourceProperty.GetValue(source, null), null);
                }
                catch (Exception)
                {
                    Console.WriteLine(@"QAZlpQtSbuxuePR89Znk");
                }
            }
        }
        return destination;
    }
    #endregion

    public static List<Dictionary<string, object>> Read(DbDataReader reader)
    {
        List<Dictionary<string, object>> expandolist = new List<Dictionary<string, object>>();
        foreach (var item in reader)
        {
            IDictionary<string, object> expando = new ExpandoObject();
            foreach (PropertyDescriptor propertyDescriptor in TypeDescriptor.GetProperties(item))
            {
                var obj = propertyDescriptor.GetValue(item);
                expando.Add(propertyDescriptor.Name, obj);
            }
            expandolist.Add(new Dictionary<string, object>(expando));
        }
        return expandolist;
    }

    public static Dictionary<string, object> DictionaryFromType(this object atype)
    {
        if (atype == null) return new Dictionary<string, object>();
        Type t = atype.GetType();
        PropertyInfo[] props = t.GetProperties();
        Dictionary<string, object> dict = new Dictionary<string, object>();
        foreach (PropertyInfo prp in props)
        {
            object value = prp.GetValue(atype, new object[] { });
            dict.Add(prp.Name, value);
        }
        return dict;
    }

    public static string[] PropertiesFromType(this object atype)
    {
        if (atype == null) return new string[] { };
        Type t = atype.GetType();
        PropertyInfo[] props = t.GetProperties();
        List<string> propNames = new List<string>();
        foreach (PropertyInfo prp in props)
        {
            propNames.Add(prp.Name);
        }
        return propNames.ToArray();
    }



    public static string GetMyTable<T>(this IEnumerable<T> list, params Expression<Func<T, object>>[] fxns)
    {

        StringBuilder sb = new StringBuilder();
        sb.Append("<TABLE>\n");

        sb.Append("<TR>\n");
        foreach (var fxn in fxns)
        {
            sb.Append("<TD>");
            sb.Append(GetName(fxn));
            sb.Append("</TD>");
        }
        sb.Append("</TR> <!-- HEADER -->\n");


        foreach (var item in list)
        {
            sb.Append("<TR>\n");
            foreach (var fxn in fxns)
            {
                sb.Append("<TD>");
                sb.Append(fxn.Compile()(item));
                sb.Append("</TD>");
            }
            sb.Append("</TR>\n");
        }
        sb.Append("</TABLE>");

        return sb.ToString();
    }

    public static string GetName<T>(Expression<Func<T, object>> expr)
    {
        var member = expr.Body as MemberExpression;
        if (member != null)
            return Helpers.GetName2(member);

        var unary = expr.Body as UnaryExpression;
        if (unary != null)
            return GetName2((MemberExpression)unary.Operand);

        return "?+?";
    }

    public static string GetName2(MemberExpression member)
    {
        var fieldInfo = member.Member as FieldInfo;
        if (fieldInfo != null)
        {
            var d = fieldInfo.GetCustomAttribute(typeof(DescriptionAttribute)) as DescriptionAttribute;
            if (d != null) return d.Description;
            return fieldInfo.Name;
        }

        var propertInfo = member.Member as PropertyInfo;
        if (propertInfo != null)
        {
            var d = propertInfo.GetCustomAttribute(typeof(DescriptionAttribute)) as DescriptionAttribute;
            if (d != null) return d.Description;
            return propertInfo.Name;
        }

        return "?-?";
    }

    public static string ToEn(this string text)
    {
        return String.Join("", text.Normalize(NormalizationForm.FormD)
        .Where(c => char.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark));
    }

    public static string toTr(this string str)
    {
        string once, sonra;
        once = str;
        sonra = once.Replace('ı', 'i');
        once = sonra.Replace('ö', 'o');
        sonra = once.Replace('ü', 'u');
        once = sonra.Replace('ş', 's');
        sonra = once.Replace('ğ', 'g');
        once = sonra.Replace('ç', 'c');
        sonra = once.Replace('İ', 'I');
        once = sonra.Replace('Ö', 'O');
        sonra = once.Replace('Ü', 'U');
        once = sonra.Replace('Ş', 'S');
        sonra = once.Replace('Ğ', 'G');
        once = sonra.Replace('Ç', 'C');
        str = once;
        return str;

    }

    public static string toCustomTr(this string str)
    {
        str = str.Trim().Replace(".", "").Replace(" ", "").ToLower(new CultureInfo("tr-TR", false)).toTr();
        return str;
    }

    public static void setUrl(this string url)
    {
        System.Diagnostics.Process.Start(url);
    }

    public static string getStr(this string str, string start, string end)
    {
        int first = str.IndexOf(start);
        int last = str.LastIndexOf(end);
        string str2 = str.Substring(first + 1, last - first - 1);
        return str2;
    }

    public static double ToMaxMin(bool durum, params double[] args)
    {
        return durum ? args.ToList().Max() : args.ToList().Min();
    }

    public static List<KeyValuePair<string, string>> GetEnumValuesAndDescriptions<T>()
    {
        Type enumType = typeof(T);

        if (enumType.BaseType != typeof(Enum))
            throw new ArgumentException("T is not System.Enum");

        List<KeyValuePair<string, string>> enumValList = new List<KeyValuePair<string, string>>();

        foreach (var e in Enum.GetValues(typeof(T)))
        {
            var fi = e.GetType().GetField(e.ToString());
            var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            enumValList.Add(new KeyValuePair<string, string>((attributes.Length > 0) ? e.ToString() : e.ToString(), attributes[0].Description));
        }

        return enumValList;
    }

    public static List<string> GetEnumDescriptions<T>()
    {
        Type enumType = typeof(T);

        if (enumType.BaseType != typeof(Enum))
            throw new ArgumentException("T is not System.Enum");

        List<string> enumValList = new List<string>();

        foreach (var e in Enum.GetValues(typeof(T)))
        {
            var fi = e.GetType().GetField(e.ToString());
            var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            enumValList.Add(attributes[0].Description);
        }

        return enumValList;
    }




    //public static Object GetPropValue(this Object obj, String name)
    //{
    //    foreach (String part in name.Split('.'))
    //    {
    //        if (obj == null) { return null; }

    //        Type type = obj.GetType();
    //        PropertyInfo info = type.GetProperty(part);
    //        if (info == null) { return null; }

    //        obj = info.GetValue(obj, null);
    //    }
    //    return obj;
    //}



    public static void noSetValueCustom<T>(this object container, string nopropertyName, object value, object total)
    {
        var prop = container.GetType().GetProperties().ToList();
        prop.ForEach(o =>
        {
            if (!nopropertyName.Contains(o.Name) && o.CanWrite)
            {
                var val = container.GetPropValue(o.Name);
                if (val != null)
                    container.SetValueCustom(o.Name, (((val.ToDouble() * value.ToDouble())) / total.ToDouble()).ToString("0.##"));
            }
            else
            {
                var val = container.GetPropValue(o.Name);
                if (val != null)
                    container.SetValueCustom(o.Name, val);
            }
        });
    }

    public static void noSetValueCustom2<T>(this object container2, List<T> containerList, string nopropertyName, string propertyName)
    {

        container2.GetType().GetProperties().ToList().ForEach(o =>
        {
            if (!nopropertyName.Contains(o.Name) && (!string.IsNullOrEmpty(propertyName) ? propertyName.Contains(o.Name) : true) && o.CanWrite)
            {
                var total = new List<double>();
                containerList.Where(l => l.GetType().GetProperty(o.Name).CanWrite).ToList().ForEach(oo =>
                {
                    var vals = oo.GetPropValue(o.Name).ToDouble();
                    total.Add(vals);
                });
                var val = container2.GetPropValue(o.Name).ToDouble();
                total.Add(val);
                if (total.Count > 0)
                    container2.SetValueCustom(o.Name, (total.Average()).ToString("0.##"));
            }
            else
            {
                containerList.ForEach(oo =>
                {
                    var vals = oo.GetPropValue(o.Name);
                    container2.SetValueCustom(o.Name, vals);
                });
            }
        });
    }



    public static string toFiligran(this string path)
    {
        if (string.IsNullOrEmpty(path)) return "";
        var strSlash = "\\";
        var aa = path.Split('\\');
        if (aa.Length < 2)
        {
            strSlash = "/";
            aa = path.Split(strSlash);
        }
        if (aa.Length < 2)
        {
            strSlash = "//";
            aa = path.Split(strSlash);
        }

        var bb = aa[path.Split(strSlash).Count() - 1];
        //var cc = bb.Split('.')[0] + "_filigran" + "." + bb.Split('.')[1];
        var cc = bb.Split('.')[0] + "_filigran" + ".jpg";
        var targetPathFiligran = path.Replace(bb, cc);
        return targetPathFiligran;
    }




    public static string StripHtml(this string text)
    {
        return Regex.Replace(text, @"<(.|\n)*?>", string.Empty);
    }


    public static byte[] Serialize(this object item)
    {
        var jsonString = JsonConvert.SerializeObject(item);
        return Encoding.UTF8.GetBytes(jsonString);
    }

    public static T Deserialize<T>(this string serializedObject, string format)
    {
        if (serializedObject == null)
            return default(T);

        var dateTimeConverter = new IsoDateTimeConverter { DateTimeFormat = format };

        return JsonConvert.DeserializeObject<T>(serializedObject, dateTimeConverter);
    }

    public static T Deserialize<T>(this string serializedObject)
    {
        if (serializedObject == null)
            return default(T);

        return JsonConvert.DeserializeObject<T>(serializedObject);
    }

    public static string ToJson(this object model)
    {
        var jsonString = JsonConvert.SerializeObject(model);
        return jsonString;
    }


    public static async Task ForEachAsync<T>(this List<T> list, Func<T, Task> func)
    {
        foreach (var value in list)
        {
            await func(value);
        }
    }


    public static string ToQueryStringCustom(this object request, string separator = ",")
    {
        if (request == null)
            throw new ArgumentNullException("request");

        // Get all properties on the object
        var properties = request.GetType().GetProperties()
            .Where(x => x.CanRead)
            .Where(x => x.GetValue(request, null) != null)
            .ToDictionary(x => x.Name, x => x.GetValue(request, null));

        // Get names for all IEnumerable properties (excl. string)
        var propertyNames = properties
            .Where(x => !(x.Value is string) && x.Value is IEnumerable)
            .Select(x => x.Key)
            .ToList();

        // Concat all IEnumerable properties into a comma separated string
        foreach (var key in propertyNames)
        {
            var valueType = properties[key].GetType();
            var valueElemType = valueType.IsGenericType
                                    ? valueType.GetGenericArguments()[0]
                                    : valueType.GetElementType();
            if (valueElemType.IsPrimitive || valueElemType == typeof(string))
            {
                var enumerable = properties[key] as IEnumerable;
                properties[key] = string.Join(separator, enumerable.Cast<object>());
            }
        }

        // Concat all key/value pairs into a string separated by ampersand
        return string.Join("&", properties
            .Select(x => string.Concat(
                Uri.EscapeDataString(x.Key), "=",
                Uri.EscapeDataString(x.Value.ToString()))));
    }



    public static bool isMail(this string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return !(addr.Address == email);
        }
        catch
        {
            return false;
        }
    }

    public static string limit(this string source, int maxLength)
    {
        if (source.Length <= maxLength)
        {
            return source;
        }

        return source.Substring(0, maxLength);
    }

}


public class ParameterBinder : ExpressionVisitor
{
    public ParameterExpression value;
    protected override Expression VisitParameter(ParameterExpression node)
    {
        return node.Type == value.Type ? value : base.VisitParameter(node);
    }
}
