/*
Copyright (c) 2003-2012, CKSource - Frederico Knabben. All rights reserved.
For licensing, see LICENSE.html or http://ckfinder.com/license

Cấu hình CKFinder cho .NET Core
Thay thế config.ascx (ASP.NET Web Forms) bằng JavaScript configuration
*/

CKFinder.customConfig = function( config ) {
    // Kích thước popup
    config.height = 600;
    config.width = 800;
    
    // Ngôn ngữ
    config.language = 'vi';
    
    // Connector URL - sử dụng controller thay vì connector.aspx
    config.connectorPath = '/admin/ckfinder/core/connector/aspx/connector.aspx';
    
    // Base URL cho files
    var baseUrl = window.location.protocol + '//' + window.location.host + '/upload/';
    
    // Resource Types
    config.resourceType = 'Images';
    
    // Cấu hình upload
    config.uploadCheckImages = true;
    config.uploadMaxSize = 0; // Không giới hạn (có thể set giá trị cụ thể)
    
    // Allowed extensions cho Images
    config.allowedExtensions = 'bmp,gif,jpeg,jpg,png';
    config.deniedExtensions = '';
    
    // Thumbnail settings
    config.thumbnails = {
        enabled: true,
        directAccess: false,
        maxWidth: 100,
        maxHeight: 100,
        quality: 80
    };
    
    // Image settings
    config.images = {
        maxWidth: 1600,
        maxHeight: 1200,
        quality: 80
    };
    
    // Security settings
    config.disallowUnsafeCharacters = true;
    config.forceSingleExtension = true;
    config.secureImageUploads = true;
    
    // Hide folders
    config.hideFolders = ['.svn', 'CVS', '_thumbs'];
    
    // Hide files
    config.hideFiles = ['.*'];
    
    // HTML extensions (for security)
    config.htmlExtensions = ['html', 'htm', 'xml', 'js'];
    
    // Disable plugins không cần thiết (nếu có)
    // config.removePlugins = 'basket';
};
