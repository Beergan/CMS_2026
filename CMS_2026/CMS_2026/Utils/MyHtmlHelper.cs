using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using CMS_2026.Common;
using CMS_2026.Pages.Admin;

namespace CMS_2026.Utils
{
    /// <summary>
    /// MyHtml Helper - Tương tự MyHtml.cshtml trong everflorcom_new
    /// Sử dụng với BaseAdminPageModel
    /// </summary>
    public static class MyHtml
    {
        private static BaseAdminPageModel? GetModel(IHtmlHelper html)
        {
            return html.ViewContext.ViewData.Model as BaseAdminPageModel;
        }

        public static IHtmlContent ComposeLanguageSwitch(this IHtmlHelper html)
        {
            var model = GetModel(html);
            if (model == null) return new HtmlString("");

            var htmlContent = $@"
<div class=""form-group row"">
    <label for=""LangIdCompose"" class=""col-12 col-md-2 col-form-label"">
        {model.Text["Input language", "Ngôn ngữ nhập"]}
    </label>
    <div class=""col-12 col-md-10"">
        <div class=""input-group"">
            <input type=""text"" class=""form-control"" id=""LangIdCompose"" 
                   value=""{model.LangIdCompose} ({model.LangCompose?.Title ?? ""})"" disabled=""disabled"" />
            <div class=""input-group-append"">
                <a href=""{Common.Constants.Admin_Url}/i18n"" class=""btn btn-primary"" type=""button"">
                    <i class=""fa-exchange""></i> {model.Text["Change", "Thay đổi"]}
                </a>
            </div>
        </div>
    </div>
</div>";

            return new HtmlString(htmlContent);
        }

        public static IHtmlContent TextBoxFor(this IHtmlHelper html, Field field, int index = -1, string value = "", bool required = false, string pattern = "", bool disabled = false)
        {
            var model = GetModel(html);
            if (model == null) return new HtmlString("");

            var fieldId = index > -1 ? $"{field.Name}-{index}" : field.Name;
            var requiredAttr = required ? "required" : "";
            var disabledAttr = disabled ? "disabled" : "";
            var patternAttr = !string.IsNullOrEmpty(pattern) ? $"data-toggle=input-mask data-mask-format={pattern}" : "";
            var requiredStar = required ? "<span class=\"text-danger\">*</span>" : "";

            var htmlContent = $@"
<div class=""form-group row"">
    <label originId=""{field.Name}"" for=""{fieldId}"" class=""col-12 col-md-2 col-form-label"">
        {model.Text[field.Title]} {requiredStar}
    </label>
    <div class=""col-12 col-md-10"">
        <input class=""form-control"" type=""text"" 
               {patternAttr}
               {requiredAttr}
               {disabledAttr}
               parsley-type=""text""
               originId=""{field.Name}""
               name=""{fieldId}""
               id=""{fieldId}""
               placeholder=""{field.Prompt}""
               value=""{value}"">
    </div>
</div>";

            return new HtmlString(htmlContent);
        }

        public static IHtmlContent TextAreaFor(this IHtmlHelper html, Field field, int index = -1, string value = "", bool required = false, bool useRichText = false)
        {
            var model = GetModel(html);
            if (model == null) return new HtmlString("");

            var fieldId = index > -1 ? $"{field.Name}-{index}" : field.Name;
            var requiredAttr = required ? "required" : "";
            var requiredStar = required ? "<span class=\"text-danger\">*</span>" : "";
            var richTextClass = useRichText ? "form-control myckeditor" : "form-control";
            var richTextAttr = useRichText ? "myckeditor" : "";
            
            // For textarea, we need to escape HTML to prevent breaking the textarea tag
            // But for CKEditor (useRichText=true), the value should contain HTML
            // CKEditor will handle the HTML rendering, so we just need to escape special characters
            // that could break the textarea tag itself (like </textarea>)
            var escapedValue = WebUtility.HtmlEncode(value);

            var htmlContent = $@"
<div class=""form-group row"">
    <label originId=""{field.Name}"" for=""{fieldId}"" class=""col-12 col-md-2 col-form-label"">
        {model.Text[field.Title]} {requiredStar}
    </label>
    <div class=""col-12 col-md-10"">
        <textarea class=""{richTextClass}"" 
                  {requiredAttr}
                  parsley-type=""text""
                  originId=""{field.Name}""
                  name=""{fieldId}""
                  id=""{fieldId}""
                  placeholder=""{field.Prompt}"" rows=""7"" {richTextAttr}>{escapedValue}</textarea>
    </div>
</div>";

            return new HtmlString(htmlContent);
        }

        public static IHtmlContent RadioFor(this IHtmlHelper html, Field field, int index = -1, bool value = false)
        {
            var model = GetModel(html);
            if (model == null) return new HtmlString("");

            var fieldId = index > -1 ? $"{field.Name}-{index}" : field.Name;
            var checkedAttr = value ? "checked" : "";

            var htmlContent = $@"
<div class=""form-group row"">
    <label originId=""{field.Name}"" class=""col-12 col-md-2 col-form-label"" for=""{fieldId}"">
        {model.Text[field.Title]}
    </label>
    <div class=""col-12 col-md-10"">
        <input type=""checkbox"" originId=""{field.Name}"" name=""{fieldId}"" 
               id=""{fieldId}"" value=""true"" {checkedAttr}
               data-plugin=""switchery"" data-color=""#3ec396"" />
    </div>
</div>";

            return new HtmlString(htmlContent);
        }

        public static IHtmlContent ComboboxFor(this IHtmlHelper html, Field field, int index = -1, Dictionary<string, string>? options = null, string? value = null, bool disabled = false)
        {
            var model = GetModel(html);
            if (model == null) return new HtmlString("");

            var fieldId = index > -1 ? $"{field.Name}-{index}" : field.Name;
            var disabledAttr = disabled ? "disabled" : "";
            
            var optionsHtml = "";
            if (options != null)
            {
                foreach (var item in options)
                {
                    var selected = item.Key == value ? "selected" : "";
                    optionsHtml += $"<option value=\"{item.Key}\" {selected}>{model.Text[item.Value]}</option>\n";
                }
            }

            var htmlContent = $@"
<div class=""form-group row"">
    <label originId=""{field.Name}"" class=""col-12 col-md-2 col-form-label"" for=""{fieldId}"">
        {model.Text[field.Title]}
    </label>
    <div class=""col-12 col-md-10"">
        <select class=""form-control"" originId=""{field.Name}"" id=""{fieldId}""
                name=""{fieldId}"" {disabledAttr}>
            {optionsHtml}
        </select>
    </div>
</div>";

            return new HtmlString(htmlContent);
        }

        public static IHtmlContent NumberBoxFor(this IHtmlHelper html, Field field, int index = -1, object? value = null, bool required = false, string pattern = "", bool disabled = false)
        {
            var model = GetModel(html);
            if (model == null) return new HtmlString("");

            var fieldId = index > -1 ? $"{field.Name}-{index}" : field.Name;
            var requiredAttr = required ? "required" : "";
            var disabledAttr = disabled ? "disabled" : "";
            var patternAttr = !string.IsNullOrEmpty(pattern) ? $"data-toggle=input-mask data-mask-format={pattern}" : "";
            var requiredStar = required ? "<span class=\"text-danger\">*</span>" : "";
            var valueStr = value?.ToString() ?? "0";

            var htmlContent = $@"
<div class=""form-group row"">
    <label originId=""{field.Name}"" for=""{fieldId}"" class=""col-12 col-md-2 col-form-label"">
        {model.Text[field.Title]} {requiredStar}
    </label>
    <div class=""col-12 col-md-10"">
        <input class=""form-control"" type=""number"" 
               {patternAttr}
               {requiredAttr}
               {disabledAttr}
               parsley-type=""number""
               originId=""{field.Name}""
               name=""{fieldId}""
               id=""{fieldId}""
               placeholder=""{field.Prompt}""
               value=""{valueStr}"">
    </div>
</div>";

            return new HtmlString(htmlContent);
        }

        public static IHtmlContent DateTimeFor(this IHtmlHelper html, Field field, int index = -1, DateTime? value = null, bool required = false, string pattern = "", bool disabled = false)
        {
            var model = GetModel(html);
            if (model == null) return new HtmlString("");

            var fieldId = index > -1 ? $"{field.Name}-{index}" : field.Name;
            var requiredAttr = required ? "required" : "";
            var disabledAttr = disabled ? "disabled" : "";
            var requiredStar = required ? "<span class=\"text-danger\">*</span>" : "";
            var valueStr = value.HasValue ? $"{value:yyyy-MM-ddTHH:mm:ss}" : "";

            var htmlContent = $@"
<div class=""form-group row"">
    <label originId=""{field.Name}"" for=""{fieldId}"" class=""col-12 col-md-2 col-form-label"">
        {model.Text[field.Title]} {requiredStar}
    </label>
    <div class=""col-12 col-md-10"">
        <input class=""form-control"" type=""datetime-local"" 
               {requiredAttr}
               {disabledAttr}
               originId=""{field.Name}""
               name=""{fieldId}""
               id=""{fieldId}""
               placeholder=""{field.Prompt}""
               value=""{valueStr}"">
    </div>
</div>";

            return new HtmlString(htmlContent);
        }

        public static IHtmlContent TagsInputFor(this IHtmlHelper html, Field field, int index = -1, string value = "", bool required = false, bool disabled = false)
        {
            var model = GetModel(html);
            if (model == null) return new HtmlString("");

            var fieldId = index > -1 ? $"{field.Name}-{index}" : field.Name;
            var requiredAttr = required ? "required" : "";
            var disabledAttr = disabled ? "disabled" : "";
            var requiredStar = required ? "<span class=\"text-danger\">*</span>" : "";

            var htmlContent = $@"
<div class=""form-group row"">
    <label originId=""{field.Name}"" for=""{fieldId}"" class=""col-12 col-md-2 col-form-label"">
        {model.Text[field.Title]} {requiredStar}
    </label>
    <div class=""col-12 col-md-10"">
        <input class=""form-control"" type=""text"" 
               {requiredAttr}
               {disabledAttr}
               data-role=""tagsinput""
               originId=""{field.Name}""
               name=""{fieldId}""
               id=""{fieldId}""
               placeholder=""{field.Prompt}""
               value=""{value}"">
    </div>
</div>";

            return new HtmlString(htmlContent);
        }

        public static IHtmlContent ImagePickerFor(this IHtmlHelper html, Field field, int index = -1, string value = "", bool required = false)
        {
            var model = GetModel(html);
            if (model == null) return new HtmlString("");

            var fieldId = index > -1 ? $"{field.Name}-{index}" : field.Name;
            var requiredAttr = required ? "required" : "";
            var requiredStar = required ? "<span class=\"text-danger\">*</span>" : "";
            var imageSrc = string.IsNullOrEmpty(value) ? "/assets/images/no-image.jpg" : value;

            var htmlContent = $@"
<div class=""form-group row"">
    <label originid=""{field.Name}"" for=""{fieldId}"" class=""col-12 col-md-2 col-form-label"">
        {model.Text[field.Title]} {requiredStar}
    </label>
    <div class=""col-12 col-md-10"">
        <div class=""input-group"">
            <input type=""text"" class=""form-control"" 
                   {requiredAttr}
                   name=""{fieldId}""
                   id=""{fieldId}""
                   originid=""{field.Name}""
                   value=""{value ?? ""}""
                   readonly />
            <div class=""input-group-append"">
                <button originid=""{field.Name}"" target=""{fieldId}""
                        class=""btn btn-primary waves-effect waves-light btn-ckfinder-popup""
                        type=""button"">
                    <i class=""fe-upload""></i> {model.Text["Browse files ..", "Duyệt file .."]}
                </button>
            </div>
        </div>
        <img id=""img-{fieldId}"" originid=""{field.Name}"" 
             src=""{imageSrc}"" 
             style=""margin-top:10px; max-height: 250px; max-width: 100%"" />
    </div>
</div>";

            return new HtmlString(htmlContent);
        }

        public static IHtmlContent ListComboboxFor(this IHtmlHelper html, Field field, Dictionary<string, string>? options = null, string? value = null, bool disabled = false)
        {
            var model = GetModel(html);
            if (model == null) return new HtmlString("");

            var ids = !string.IsNullOrEmpty(value)
                ? value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                : new string[] { };
            var disabledAttr = disabled ? "disabled" : "";
            var optionsJson = options != null ? System.Text.Json.JsonSerializer.Serialize(options).Replace("\"", "&quot;") : "{}";

            var htmlContent = "<div id=\"comboboxlist\">";
            
            for (var i = 0; i < ids.Length; i++)
            {
                var optionsHtml = "";
                if (options != null)
                {
                    foreach (var item in options)
                    {
                        var selected = item.Key == ids[i] ? "selected" : "";
                        optionsHtml += $"<option value=\"{item.Key}\" {selected}>{model.Text[item.Value]}</option>\n";
                    }
                }
                
                htmlContent += $@"
<div class=""form-group row comboboxlist"">
    <label originId=""{field.Name}"" class=""col-12 col-md-2 col-form-label"" for=""{field.Name}"">
        {model.Text[field.Title]}
    </label>
    <div class=""col-12 col-md-9"">
        <select class=""form-control"" originId=""{field.Name}"" id=""{field.Name}"" name=""{field.Name}"" {disabledAttr}>
            {optionsHtml}
        </select>
    </div>
    <div class=""col-md-1"">
        <div class=""btn-group"">
            <button type=""button"" class=""btn btn-danger btn-sm delete-btn"" onclick=""deleteCat(this)"">
                <i class=""fa fa-trash""></i>
            </button>
        </div>
    </div>
</div>
<div class=""row"">
    <div class=""col-md-11""></div>
</div>";
            }
            
            if (ids.Length == 0)
            {
                var optionsHtml = "";
                if (options != null)
                {
                    foreach (var item in options)
                    {
                        var selected = item.Key == value ? "selected" : "";
                        optionsHtml += $"<option value=\"{item.Key}\" {selected}>{model.Text[item.Value]}</option>\n";
                    }
                }
                
                htmlContent += $@"
<div class=""form-group row comboboxlist"">
    <label originId=""{field.Name}"" class=""col-12 col-md-2 col-form-label"" for=""{field.Name}"">
        {model.Text[field.Title]}
    </label>
    <div class=""col-12 col-md-10"">
        <select class=""form-control"" originId=""{field.Name}"" id=""{field.Name}"" name=""{field.Name}"" {disabledAttr}>
            {optionsHtml}
        </select>
    </div>
</div>
<div class=""row"">
    <div class=""col-md-11""></div>
    <div class=""col-md-1"">
        <div class=""btn-group"">
            <button type=""button"" class=""btn btn-danger btn-sm delete-btn"" onclick=""deleteCat(this)"">
                <i class=""fa fa-trash""></i>
            </button>
        </div>
    </div>
</div>";
            }
            
            htmlContent += $@"</div>
<div class=""row"">
    <div class=""col-lg-2"">
        <button type=""button"" class=""btn btn-primary"" onclick=""addCat('{field.Name}', '{field.Title}', '{optionsJson}')"">Thêm mục</button>
    </div>
</div>";

            return new HtmlString(htmlContent);
        }

        public static IHtmlContent ImagePickerForlist(this IHtmlHelper html, Field field, string value = "", bool required = false)
        {
            var model = GetModel(html);
            if (model == null) return new HtmlString("");

            var imageUrls = !string.IsNullOrEmpty(value)
                ? value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                : new string[] { };
            
            var htmlContent = "<div id=\"image-picker-container\">";
            
            for (var i = 0; i < imageUrls.Length; i++)
            {
                htmlContent += $@"
<div class=""form-group row image-picker-item"">
    <label for=""{field.Name}-{i}"" class=""col-12 col-md-2 col-form-label"">
        {model.Text[field.Title]}
    </label>
    <div class=""col-12 col-md-8"">
        <div class=""input-group"">
            <input type=""text"" class=""form-control ckfinder-input"" name=""{field.Name}"" id=""{field.Name}-{i}""
                   value=""{imageUrls[i]}"" readonly />
            <div class=""input-group-append"">
                <button originid=""{field.Name}"" target=""{field.Name}-{i}""
                        class=""btn btn-primary waves-effect waves-light btn-ckfinder-popup""
                        type=""button"" onclick=""openCKFinder(this)"">
                    <i class=""fe-upload""></i> {model.Text["Browse files ..", "Duyệt file"]}
                </button>
            </div>
        </div>
        <img id=""img-{field.Name}-{i}"" originid=""{field.Name}"" src=""{imageUrls[i]}"" 
             class=""image-preview"" style=""margin-top:10px; max-height: 250px; max-width: 100%"" />
    </div>
    <div class=""col-md-2"">
        <div class=""btn-group"">
            <button type=""button"" class=""btn btn-info btn-sm move-up-btn"" onclick=""moveUpImage(this)"">
                <i class=""fa fa-arrow-up""></i>
            </button>
            <button type=""button"" class=""btn btn-info btn-sm move-down-btn"" onclick=""moveDownImage(this)"">
                <i class=""fa fa-arrow-down""></i>
            </button>
            <button type=""button"" class=""btn btn-danger btn-sm delete-btn"" onclick=""deleteImage(this)"">
                <i class=""fa fa-trash""></i>
            </button>
        </div>
    </div>
</div>";
            }
            
            if (imageUrls.Length == 0)
            {
                htmlContent += $@"
<div class=""form-group row image-picker-item"">
    <label for=""{field.Name}"" class=""col-12 col-md-2 col-form-label"">
        {model.Text[field.Title]}
    </label>
    <div class=""col-12 col-md-8"">
        <div class=""input-group"">
            <input type=""text"" class=""form-control ckfinder-input"" name=""{field.Name}"" id=""{field.Name}""
                   value="""" readonly />
            <div class=""input-group-append"">
                <button originid=""{field.Name}"" target=""{field.Name}""
                        class=""btn btn-primary waves-effect waves-light btn-ckfinder-popup""
                        type=""button"" onclick=""openCKFinder(this)"">
                    <i class=""fe-upload""></i> {model.Text["Browse files ..", "Duyệt file"]}
                </button>
            </div>
        </div>
        <img id=""img-{field.Name}"" originid=""{field.Name}"" src=""/assets/images/no-image.jpg"" 
             class=""image-preview"" style=""margin-top:10px; max-height: 250px; max-width: 100%"" />
    </div>
    <div class=""col-md-2"">
        <div class=""btn-group"">
            <button type=""button"" class=""btn btn-info btn-sm move-up-btn"" onclick=""moveUpImage(this)"">
                <i class=""fa fa-arrow-up""></i>
            </button>
            <button type=""button"" class=""btn btn-info btn-sm move-down-btn"" onclick=""moveDownImage(this)"">
                <i class=""fa fa-arrow-down""></i>
            </button>
            <button type=""button"" class=""btn btn-danger btn-sm delete-btn"" onclick=""deleteImage(this)"">
                <i class=""fa fa-trash""></i>
            </button>
        </div>
    </div>
</div>";
            }
            
            htmlContent += $@"</div>
<button type=""button"" class=""btn btn-primary"" onclick=""addImagePicker('{field.Name}', '{field.Title}')"">Thêm hình ảnh</button>";

            return new HtmlString(htmlContent);
        }
    }
}
