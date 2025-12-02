using System.Collections.Generic;
using System.Linq;

namespace CMS_2026.Services
{
    /// <summary>
    /// Translator service tương đương với MyTranslator và AdminTranslator trong everflorcom_new
    /// </summary>
    public class TranslatorService
    {
        private readonly string _langId;
        private readonly IDictionary<string, string>? _phrases;

        public TranslatorService(string langId, IDictionary<string, string>? phrases = null)
        {
            _langId = langId;
            _phrases = phrases;
        }

        /// <summary>
        /// Get translated text (en, vi) - Tương đương với MyTranslator[en, vi]
        /// </summary>
        public string this[string en, string vi]
        {
            get
            {
                if (_langId == "en")
                    return en;

                if (_langId == "vi")
                    return vi;

                // For other languages, try to get from phrases dictionary
                return _phrases?.ContainsKey(en) == true ? _phrases[en] : en;
            }
        }

        /// <summary>
        /// Get translated text from format "en:English|vi:Tiếng Việt" - Tương đương với AdminTranslator[str]
        /// </summary>
        public string this[string str]
        {
            get
            {
                if (string.IsNullOrEmpty(str))
                    return str;

                // Format: "en:English|vi:Tiếng Việt"
                if (str.Contains("|") && str.Contains(":"))
                {
                    var parts = str.Split('|');
                    foreach (var part in parts)
                    {
                        if (part.StartsWith($"{_langId}:"))
                        {
                            return part.Substring($"{_langId}:".Length);
                        }
                    }

                    // Fallback: try to get en or vi
                    if (_langId == "en")
                    {
                        var enPart = parts.FirstOrDefault(p => p.StartsWith("en:"));
                        if (enPart != null)
                            return enPart.Substring("en:".Length);
                    }
                    else
                    {
                        var viPart = parts.FirstOrDefault(p => p.StartsWith("vi:"));
                        if (viPart != null)
                            return viPart.Substring("vi:".Length);
                    }
                }

                return str;
            }
        }
    }

    /// <summary>
    /// Frontend translator - Tương đương với MyTranslator
    /// Lấy LangId từ route/cookie
    /// </summary>
    public class FrontendTranslator
    {
        private readonly string _langId;
        private readonly IDictionary<string, string>? _phrases;

        public FrontendTranslator(string langId, IDictionary<string, string>? phrases = null)
        {
            _langId = langId ?? "vi";
            _phrases = phrases;
        }

        /// <summary>
        /// Get translated text (en, vi)
        /// </summary>
        public string this[string en, string vi]
        {
            get
            {
                if (_langId == "en")
                    return en;

                if (_langId == "vi")
                    return vi;

                // For other languages, try to get from phrases dictionary
                return _phrases?.ContainsKey(en) == true ? _phrases[en] : en;
            }
        }
    }

    /// <summary>
    /// Admin translator - Tương đương với AdminTranslator
    /// Dùng LangIdDisplay cho admin panel
    /// </summary>
    public class AdminTranslator
    {
        private readonly string _langId;

        public AdminTranslator(string langId)
        {
            _langId = langId ?? "vi";
        }

        /// <summary>
        /// Get translated text (en, vi)
        /// </summary>
        public string this[string en, string vi] => _langId == "en" ? en : vi;

        /// <summary>
        /// Get translated text from format "en:English|vi:Tiếng Việt"
        /// </summary>
        public string this[string str]
        {
            get
            {
                if (string.IsNullOrEmpty(str))
                    return str;

                // Format: "en:English|vi:Tiếng Việt"
                if (str.Contains("|") && str.Contains(":"))
                {
                    var parts = str.Split('|');
                    foreach (var part in parts)
                    {
                        if (part.StartsWith($"{_langId}:"))
                        {
                            return part.Substring($"{_langId}:".Length);
                        }
                    }

                    // Fallback: try to get en or vi
                    if (_langId == "en")
                    {
                        var enPart = parts.FirstOrDefault(p => p.StartsWith("en:"));
                        if (enPart != null)
                            return enPart.Substring("en:".Length);
                    }
                    else
                    {
                        var viPart = parts.FirstOrDefault(p => p.StartsWith("vi:"));
                        if (viPart != null)
                            return viPart.Substring("vi:".Length);
                    }
                }

                return str;
            }
        }
    }
}

