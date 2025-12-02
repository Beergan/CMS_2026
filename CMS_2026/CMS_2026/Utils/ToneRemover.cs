using System;

namespace CMS_2026.Utils
{
    public static class ToneRemover
    {
        private static char[] specChars = new char[] { ' ', '/', '.', ':', '%', '~', '@', '#', '$', '^', '&', '*', '(', ')', '+', 'A', 'a', 'Ă', 'ă', 'Â', 'â', 'E', 'e', 'Ê', 'ê', 'I', 'i', 'O', 'o', 'Ô', 'ô', 'Ơ', 'ơ', 'U', 'u', 'Ư', 'ư', 'Y', 'y', 'À', 'à', 'Ằ', 'ằ', 'Ầ', 'ầ', 'È', 'è', 'Ề', 'ề', 'Ì', 'ì', 'Ò', 'ò', 'Ồ', 'ồ', 'Ờ', 'ờ', 'Ù', 'ù', 'Ừ', 'ừ', 'Ỳ', 'ỳ', 'Á', 'á', 'Ắ', 'ắ', 'Ấ', 'ấ', 'É', 'é', 'Ế', 'ế', 'Í', 'í', 'Ó', 'ó', 'Ố', 'ố', 'Ớ', 'ớ', 'Ú', 'ú', 'Ứ', 'ứ', 'Ý', 'ý', 'Ả', 'ả', 'Ẳ', 'ẳ', 'Ẩ', 'ẩ', 'Ẻ', 'ẻ', 'Ể', 'ể', 'Ỉ', 'ỉ', 'Ỏ', 'ỏ', 'Ổ', 'ổ', 'Ở', 'ở', 'Ủ', 'ủ', 'Ử', 'ử', 'Ỷ', 'ỷ', 'Ã', 'ã', 'Ẵ', 'ẵ', 'Ẫ', 'ẫ', 'Ẽ', 'ẽ', 'Ễ', 'ễ', 'Ĩ', 'ĩ', 'Õ', 'õ', 'Ỗ', 'ỗ', 'Ỡ', 'ỡ', 'Ũ', 'ũ', 'Ữ', 'ữ', 'Ỹ', 'ỹ', 'Ạ', 'ạ', 'Ặ', 'ặ', 'Ậ', 'ậ', 'Ẹ', 'ẹ', 'Ệ', 'ệ', 'Ị', 'ị', 'Ọ', 'ọ', 'Ộ', 'ộ', 'Ợ', 'ợ', 'Ụ', 'ụ', 'Ự', 'ự', 'Ỵ', 'ỵ', 'Đ', 'đ' };
        private static char[] slugChars = new char[] { '-', '-', '-', '-', '-', '-', '-', '-', '-', '-', '-', '-', '-', '-', '-', 'A', 'a', 'A', 'a', 'A', 'a', 'E', 'e', 'E', 'e', 'I', 'i', 'O', 'o', 'O', 'o', 'O', 'o', 'U', 'u', 'U', 'u', 'Y', 'y', 'A', 'a', 'A', 'a', 'A', 'a', 'E', 'e', 'E', 'e', 'I', 'i', 'O', 'o', 'O', 'o', 'O', 'o', 'U', 'u', 'U', 'u', 'Y', 'y', 'A', 'a', 'A', 'a', 'A', 'a', 'E', 'e', 'E', 'e', 'I', 'i', 'O', 'o', 'O', 'o', 'O', 'o', 'U', 'u', 'U', 'u', 'Y', 'y', 'A', 'a', 'A', 'a', 'A', 'a', 'E', 'e', 'E', 'e', 'I', 'i', 'O', 'o', 'O', 'o', 'O', 'o', 'U', 'u', 'U', 'u', 'Y', 'y', 'A', 'a', 'A', 'a', 'A', 'a', 'E', 'e', 'E', 'e', 'I', 'i', 'O', 'o', 'O', 'o', 'O', 'o', 'U', 'u', 'U', 'u', 'Y', 'y', 'A', 'a', 'A', 'a', 'A', 'a', 'E', 'e', 'E', 'e', 'I', 'i', 'O', 'o', 'O', 'o', 'O', 'o', 'U', 'u', 'U', 'u', 'Y', 'y', 'D', 'd' };

        public static string MakeSlug(string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            value = value.ToLower();
            char[] chars = value.ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
                int index = Array.IndexOf(specChars, chars[i]);
                if (index == -1) continue;
                chars[i] = slugChars[index];
            }

            value = new string(chars)
                .Replace(",", string.Empty)
                .Replace("?", string.Empty)
                .Replace(";", string.Empty)
                .Replace("--", "-");

            return value;
        }
    }
}

