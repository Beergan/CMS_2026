/**
 * CKEditor 5 Initialization Helper
 * Thay thế CKEditor cũ bằng CKEditor 5
 * Tương tự như ví dụ ES modules, nhưng sử dụng window.ClassicEditor từ UMD build
 */

// Global variable to store editor instances
window.ckeditor5Instances = window.ckeditor5Instances || {};

/**
 * Initialize CKEditor 5 for elements with class 'myckeditor'
 * @param {string} elementId - ID of the textarea element
 * @param {object} options - Optional configuration options
 */
async function initializeCKEditor5(elementId, options = {}) {
    // Get ClassicEditor from window (set by ckeditor5-wrapper.js)
    var ClassicEditor = window.ClassicEditor;
    
    if (!elementId || !ClassicEditor || typeof ClassicEditor.create !== 'function') {
        console.error('CKEditor 5 not loaded or element ID missing. Make sure ckeditor5.umd.js and ckeditor5-wrapper.js are loaded.');
        if (!ClassicEditor) {
            console.log('ClassicEditor not found. Available window properties:', Object.keys(window).filter(k => k.toLowerCase().includes('ck') || k.toLowerCase().includes('editor')));
            if (window.CKEDITOR) {
                console.log('CKEDITOR object keys:', Object.keys(window.CKEDITOR).slice(0, 20));
            }
        }
        return null;
    }

    const element = document.getElementById(elementId);
    if (!element) {
        console.error('Element not found: ' + elementId);
        return null;
    }

    // Destroy existing instance if any
    if (window.ckeditor5Instances[elementId]) {
        try {
            await window.ckeditor5Instances[elementId].destroy();
            delete window.ckeditor5Instances[elementId];
        } catch (e) {
            console.error('Error destroying existing CKEditor 5 instance:', e);
        }
    }

    // Default configuration
    const defaultConfig = {
        toolbar: {
            items: [
                'heading', '|',
                'bold', 'italic', 'underline', 'strikethrough', '|',
                'fontSize', 'fontColor', 'fontBackgroundColor', '|',
                'alignment', '|',
                'numberedList', 'bulletedList', '|',
                'outdent', 'indent', '|',
                'link', 'insertImage', 'insertTable', '|',
                'sourceEditing', 'removeFormat', '|',
                'undo', 'redo'
            ],
            shouldNotGroupWhenFull: true
        },
        language: 'vi',
        image: {
            toolbar: [
                'imageTextAlternative',
                'imageStyle:inline',
                'imageStyle:block',
                'imageStyle:side'
            ]
        },
        table: {
            contentToolbar: [
                'tableColumn',
                'tableRow',
                'mergeTableCells'
            ]
        },
        simpleUpload: {
            uploadUrl: '/admin/ckfinder/core/connector/aspx/connector.aspx?command=QuickUpload&type=Images',
            withCredentials: true,
            headers: {
                'X-CSRF-TOKEN': 'CSRF-TOKEN'
            }
        },
        // CKFinder integration
        ckfinder: {
            uploadUrl: '/admin/ckfinder/core/connector/aspx/connector.aspx?command=QuickUpload&type=Images'
        }
    };

    // Merge with custom options
    const config = { ...defaultConfig, ...options };

    try {
        // Create editor instance (similar to example: ClassicEditor.create(...))
        var editor = await ClassicEditor.create(element, config);
        window.ckeditor5Instances[elementId] = editor;
        return editor;
    } catch (error) {
        console.error('Error initializing CKEditor 5 for element ' + elementId + ':', error);
        return null;
    }
}

/**
 * Initialize all CKEditor 5 instances for elements with class 'myckeditor'
 */
async function initializeAllCKEditor5() {
    const elements = document.querySelectorAll('.myckeditor');
    
    for (let i = 0; i < elements.length; i++) {
        const element = elements[i];
        const elementId = element.id || 'ckeditor5_' + Date.now() + '_' + i;
        
        if (!element.id) {
            element.id = elementId;
        }
        
        await initializeCKEditor5(elementId);
    }
}

/**
 * Destroy all CKEditor 5 instances
 */
async function destroyAllCKEditor5() {
    for (const elementId in window.ckeditor5Instances) {
        try {
            await window.ckeditor5Instances[elementId].destroy();
        } catch (e) {
            console.error('Error destroying CKEditor 5 instance:', elementId, e);
        }
    }
    window.ckeditor5Instances = {};
}

/**
 * Get editor instance by element ID
 */
function getCKEditor5Instance(elementId) {
    return window.ckeditor5Instances[elementId] || null;
}

// Auto-initialize when DOM is ready and CKEditor 5 is loaded
function setupAutoInitialize() {
    var initialized = false;
    
    function tryInitialize() {
        if (initialized) return;
        
        // Check if ClassicEditor is available (set by ckeditor5-wrapper.js)
        if (window.ClassicEditor && typeof window.ClassicEditor.create === 'function') {
            initializeAllCKEditor5();
            initialized = true;
            console.log('CKEditor 5 auto-initialized successfully');
        }
    }
    
    // Listen for CKEditor 5 loaded event (dispatched by ckeditor5-wrapper.js)
    document.addEventListener('ckeditor5:loaded', function() {
        tryInitialize();
    });
    
    // Also try to initialize after delays (fallback)
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', function() {
            // Try multiple times with increasing delays
            setTimeout(tryInitialize, 500);
            setTimeout(tryInitialize, 1000);
            setTimeout(tryInitialize, 2000);
        });
    } else {
        setTimeout(tryInitialize, 500);
        setTimeout(tryInitialize, 1000);
        setTimeout(tryInitialize, 2000);
    }
    
    // Also initialize when jQuery is ready (for compatibility)
    if (typeof jQuery !== 'undefined') {
        jQuery(document).ready(function() {
            setTimeout(tryInitialize, 500);
            setTimeout(tryInitialize, 1000);
            setTimeout(tryInitialize, 2000);
        });
    }
}

// Start setup
setupAutoInitialize();

