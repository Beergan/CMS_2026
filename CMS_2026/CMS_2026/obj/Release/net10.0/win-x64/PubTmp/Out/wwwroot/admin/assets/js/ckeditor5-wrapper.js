/**
 * CKEditor 5 Wrapper Script
 * Đảm bảo ClassicEditor có thể được truy cập từ UMD build
 * Tương tự như ví dụ ES modules, nhưng cho UMD build
 */

(function() {
    'use strict';
    
    // Function to find ClassicEditor from UMD build
    function findClassicEditor() {
        // UMD build exports to global.CKEDITOR = {} and then assigns exports to it
        // Pattern 1: Check window.CKEDITOR.ClassicEditor (most common for UMD)
        if (window.CKEDITOR && window.CKEDITOR.ClassicEditor && typeof window.CKEDITOR.ClassicEditor.create === 'function') {
            return window.CKEDITOR.ClassicEditor;
        }
        // Pattern 2: Direct window.ClassicEditor
        if (window.ClassicEditor && typeof window.ClassicEditor.create === 'function') {
            return window.ClassicEditor;
        }
        // Pattern 3: Check globalThis
        if (typeof globalThis !== 'undefined' && globalThis.CKEDITOR && globalThis.CKEDITOR.ClassicEditor && typeof globalThis.CKEDITOR.ClassicEditor.create === 'function') {
            return globalThis.CKEDITOR.ClassicEditor;
        }
        // Pattern 4: Check if UMD exports directly to window
        if (window.CKEditor && window.CKEditor.ClassicEditor && typeof window.CKEditor.ClassicEditor.create === 'function') {
            return window.CKEditor.ClassicEditor;
        }
        return null;
    }
    
    // Wait for script to load and then find ClassicEditor
    function waitForCKEditor5(maxAttempts = 100, attempt = 0) {
        if (attempt >= maxAttempts) {
            console.error('CKEditor 5 failed to load after ' + maxAttempts + ' attempts');
            console.log('Available window properties:', Object.keys(window).filter(k => k.toLowerCase().includes('ck') || k.toLowerCase().includes('editor')));
            if (window.CKEDITOR) {
                console.log('CKEDITOR object exists. Keys:', Object.keys(window.CKEDITOR).slice(0, 20));
            }
            return;
        }
        
        var ClassicEditor = findClassicEditor();
        
        if (ClassicEditor) {
            // Make sure it's available globally as window.ClassicEditor
            window.ClassicEditor = ClassicEditor;
            console.log('CKEditor 5 ClassicEditor loaded successfully');
            
            // Trigger custom event to notify that CKEditor 5 is ready
            var event = new CustomEvent('ckeditor5:loaded', { 
                detail: { ClassicEditor: ClassicEditor } 
            });
            document.dispatchEvent(event);
        } else {
            // Log debug info every 10 attempts
            if (attempt % 10 === 0) {
                console.log('Waiting for CKEditor 5... Attempt ' + attempt + '/' + maxAttempts);
                if (window.CKEDITOR) {
                    console.log('CKEDITOR exists but ClassicEditor not found. Keys:', Object.keys(window.CKEDITOR).slice(0, 20));
                }
            }
            
            // Retry after a short delay
            setTimeout(function() {
                waitForCKEditor5(maxAttempts, attempt + 1);
            }, 100);
        }
    }
    
    // Function to wait for script tag to be added and loaded
    function waitForScriptTag() {
        var scriptTags = document.querySelectorAll('script[src*="ckeditor5.umd.js"]');
        
        if (scriptTags.length === 0) {
            // Script tag not found yet, wait a bit
            setTimeout(waitForScriptTag, 100);
            return;
        }
        
        // Found script tag(s), check if loaded
        var allLoaded = true;
        scriptTags.forEach(function(script) {
            // Check if script has onload handler or is already loaded
            if (!script.hasAttribute('data-ckeditor5-loaded')) {
                allLoaded = false;
                
                // Add onload handler if not already added
                if (!script.onload && !script.hasAttribute('data-ckeditor5-handler')) {
                    script.setAttribute('data-ckeditor5-handler', 'true');
                    script.onload = function() {
                        script.setAttribute('data-ckeditor5-loaded', 'true');
                        // Wait a bit for UMD to initialize
                        setTimeout(function() {
                            waitForCKEditor5();
                        }, 100);
                    };
                    script.onerror = function() {
                        console.error('Failed to load CKEditor 5 script:', script.src);
                    };
                }
            }
        });
        
        // If all scripts are loaded, start checking for ClassicEditor
        if (allLoaded) {
            setTimeout(function() {
                waitForCKEditor5();
            }, 100);
        } else {
            // Wait a bit and check again
            setTimeout(waitForScriptTag, 100);
        }
    }
    
    // Start waiting for script tag
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', waitForScriptTag);
    } else {
        // DOM already ready
        waitForScriptTag();
    }
    
    // Also check immediately in case script is already loaded
    setTimeout(function() {
        var scriptTags = document.querySelectorAll('script[src*="ckeditor5.umd.js"]');
        if (scriptTags.length > 0) {
            var anyLoaded = false;
            scriptTags.forEach(function(script) {
                if (script.complete || script.readyState === 'complete' || script.readyState === 'loaded') {
                    anyLoaded = true;
                }
            });
            if (anyLoaded) {
                setTimeout(function() {
                    waitForCKEditor5();
                }, 200);
            }
        }
    }, 50);
})();

