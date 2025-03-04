using MediaInfo;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;
using Theresia.DO;
using Theresia.Entity;

namespace Theresia.Common
{
    public class MediaUtils
    {
        // 视频文件扩展名
        private static readonly string[] VideoExtensions =
        [
            ".mp4", ".avi", ".mkv", ".mov", ".wmv", ".flv", ".webm", ".mpg", ".mpeg", ".3gp", ".ts"
        ];

        private static readonly string[] patterns =
        {
            @"\b[a-zA-Z]+-\d+$",  // P1: AAA-11
            @"[A-Za-z]{2,}\d{2,}", // P2: AAA11
            @"\b[A-Za-z]+-[A-Za-z0-9]+\b", // P3: AAA-A11
            @"(?<!\d)(\d{6}_\d{3})(?!\d)", // P4: 032517_505
            @"^\d{4}(?!\d)",  // P5: 0302ssni131,
            @"^n\d{4}$", //p6 n0894
            @"(?<!\d)(\d{6}-\d{3})(?!\d)", // P4: 032517-505
        };

        //未修正
        private static readonly string[] uncensoredArray = { "-uncensored" };

        //标签
        private static readonly string[] labelArray = { "-1pon" };
        //网址
        private static readonly string[] addressArray = { "hjd2048.com-", "~nyap2p.com", "@蜂鳥@fengniao131.vip-", "hhd800.com@", "@蜂鳥@FENGNIAO151.VIP-", "-carib-whole" };
        //结尾
        private static readonly string[] endStringArray = { "-C", "ch" };
        //码率
        private static readonly string[] encodingFormatArray = { "h264" };
        //清晰度
        private static readonly string[] resolutionArray = { "1080p", "1080p-", "FHD", "FHD-", "HD", "HD-" };




        /// <summary>
        /// 获取视频元数据
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static MediaMetadataEntity GetMetadata(string filePath)
        {
            MediaMetadataEntity metadata = new MediaMetadataEntity();
            // 创建 MediaInfo 对象
            using (var mediaInfo = new MediaInfo.MediaInfo())
            {
                mediaInfo.Open(filePath);
                // 获取视频的分辨率（宽度和高度）
                string height = mediaInfo.Get(StreamKind.Video, 0, "Height");
                string width = mediaInfo.Get(StreamKind.Video, 0, "Width");
                // 获取视频的编码格式
                string codec = mediaInfo.Get(StreamKind.Video, 0, "CodecID");
                // 获取视频的时长
                string duration = mediaInfo.Get(StreamKind.Video, 0, "Duration");
                //视频帧率
                int frameRate = (int)Math.Round(double.Parse(mediaInfo.Get(StreamKind.Video, 0, "FrameRate")));
                int bitRate = (int)Math.Round(double.Parse(mediaInfo.Get(StreamKind.Video, 0, "BitRate")));

                int seconds = int.Parse(duration);

                string sha256 = GetFileSha256(filePath);
                
                metadata.Width = width;
                metadata.Height = height;
                metadata.Duration = seconds;
                metadata.Codec = codec;
                metadata.FrameRate = frameRate;
                metadata.BitRate = bitRate;
                metadata.Sha256 = sha256;
                metadata.FilePath = filePath;
                return metadata;
            }
        }

        // 判断文件是否是视频文件
        public static bool IsMediaFile(string filePath)
        {
            string? extension = Path.GetExtension(filePath)?.ToLower();
            if (string.IsNullOrEmpty(extension))
            {
                return false;
            }

            // 判断文件扩展名是否属于常见的视频文件扩展名之一
            foreach (var videoExtension in VideoExtensions)
            {
                if (extension.Equals(videoExtension, StringComparison.OrdinalIgnoreCase))
                {
                    return true; // 是视频文件
                }
            }

            return false; // 不是视频文件
        }

        /// <summary>
        /// 查询是否未修正
        /// </summary>
        /// <returns></returns>
        public static bool IsUncensored(string fileName)
        {
            for (var i = 0; i < uncensoredArray.Length; i++)
            {
                if (fileName.ToLower().Contains(uncensoredArray[i]))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 获取番号
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetMovieCode(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return "";
            }
            HashSet<string> result = new HashSet<string>();
            //Debug.WriteLine($"检查字符串 {fileName} 的内容");

            // 批量处理替换操作
            string data = RemoveUnwantedStrings(fileName, labelArray);
            data = RemoveUnwantedStrings(data, addressArray);
            data = RemoveSuffixIfExists(data, endStringArray);
            data = RemoveSuffixIfExists(data, encodingFormatArray);
            data = RemoveSuffixIfExists(data, resolutionArray);

            //Debug.WriteLine($"清理后文件名: {data}");

            // 检查开头四个数字
            Match check = Regex.Match(data, patterns[4]); // P5
            if (check.Success)
            {
                //Debug.WriteLine("P5 生效了");
                data = data.Substring(check.Value.Length);
            }

            // 遍历正则表达式并匹配
            foreach (var pattern in patterns)
            {
                foreach (Match match in Regex.Matches(data, pattern))
                {
                    //Debug.WriteLine($"{pattern} 生效了，匹配到: {match.Value}");
                    result.Add(match.Value);
                }
            }
            return result.Count > 0 ? FormatCode(result.First()) : "";
        }

        /// <summary>
        /// 是否有中文字幕
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool HasCaption(string fileName)
        {
            string withExtension = Path.GetFileNameWithoutExtension(fileName);
            foreach (var s in endStringArray)
            {
                if (withExtension.EndsWith(s))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 获取文件的sha256
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string GetFileSha256(string filePath)
        {
            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                int bufferSize = 1048576;
                byte[] buffer = new byte[bufferSize]; // 默认读取前 1MB
                int bytesRead = stream.Read(buffer, 0, buffer.Length);

                // 如果文件小于指定的 bufferSize，确保计算所有数据
                if (bytesRead < bufferSize)
                {
                    Array.Resize(ref buffer, bytesRead);
                }

                byte[] hashBytes = SHA256.Create().ComputeHash(buffer);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
            }
        }

        /// <summary>
        /// 格式化番号
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static string FormatCode(string input)
        {
            // 正则匹配 "字母-数字" 或 "字母数字" 形式
            string patternWithHyphen = @"^([a-zA-Z]+)-(\d+)$";  // 匹配 XXXX-111
            string patternWithoutHyphen = @"^([a-zA-Z]+)(\d+)$"; // 匹配 XXXX111

            string extraPattern = patterns[5];
            if (Regex.IsMatch(input, extraPattern))
            {
                return input;
            }
            if (Regex.IsMatch(input, patternWithHyphen)) // 如果已有 `-`
            {
                return Regex.Replace(input, patternWithHyphen, m => $"{m.Groups[1].Value.ToUpper()}-{m.Groups[2].Value}");
            }
            else if (Regex.IsMatch(input, patternWithoutHyphen)) // 如果没有 `-`
            {
                return Regex.Replace(input, patternWithoutHyphen, m => $"{m.Groups[1].Value.ToUpper()}-{m.Groups[2].Value}");
            }
            return input.ToUpper(); // 如果不符合格式，返回原字符串
        }

        /// <summary>
        /// 获取分辨率
        /// </summary>
        /// <param name="height"></param>
        /// <returns></returns>
        public static string GetResolution(string height)
        {
            //视频分辨率
            string resolution = height switch
            {
                "144" => "144P",
                "240" => "240P",
                "360" => "360P",
                "480" => "480P",
                "720" => "720P",
                "1080" => "1080P",
                "1440" => "2K",
                "2160" => "4K",
                "4320" => "8K",
                _ => height + "P"
            };
            return resolution;
        }

        /// <summary>
        /// 获取以小时为单位的时长
        /// </summary>
        /// <returns></returns>
        public static string GetDurationForChHour(int duration)
        {
            int seconds = duration;
            int hours = seconds / 3600;
            int minutes = (seconds % 3600) / 60;  // 计算分钟数
            return $"{hours}小时{minutes}分钟";
        }

        /// <summary>
        /// 移除字符串中的特定关键字
        /// </summary>
        private static string RemoveUnwantedStrings(string input, string[] patterns)
        {
            return patterns.Aggregate(input, (current, keyword) => current.Replace(keyword, ""));
        }

        /// <summary>
        /// 如果字符串以特定后缀结尾，则去除它
        /// </summary>
        private static string RemoveSuffixIfExists(string input, string[] suffixes)
        {
            foreach (var suffix in suffixes)
            {
                if (input.EndsWith(suffix))
                {
                    Debug.WriteLine($"文件名 {input} 末尾检查到 {suffix}");
                    return input.Substring(0, input.Length - suffix.Length);
                }
            }
            return input;
        }
    }
}
