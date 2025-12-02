// elFinder configuration for .NET Core
define('elFinderConfig', {
    defaultOpts: {
        url: '/admin/elfinder/connector',
        lang: 'vi',
        width: '100%',
        height: '100%',
        resizable: true,
        commandsOptions: {
            edit: {
                extraOptions: {
                    managerUrl: ''
                }
            },
            quicklook: {
                sharecadMimes: [],
                googleDocsMimes: [],
                officeOnlineMimes: []
            }
        },
        ui: ['toolbar', 'tree', 'path', 'stat'],
        uiOptions: {
            toolbar: [
                ['back', 'forward'],
                ['reload'],
                ['home', 'up'],
                ['mkdir', 'mkfile', 'upload'],
                ['open', 'download', 'getfile'],
                ['info'],
                ['quicklook'],
                ['copy', 'cut', 'paste'],
                ['rm'],
                ['duplicate', 'rename', 'edit', 'resize'],
                ['extract', 'archive'],
                ['search'],
                ['view', 'sort']
            ]
        }
    },
    managers: {
        'elfinder': {}
    }
});

