mergeInto(LibraryManager.library, {
    DownloadFile: function (arrayPtr, length, fileNamePtr) {
        var bytes = new Uint8Array(Module.HEAPU8.buffer, arrayPtr, length);
        var blob = new Blob([bytes], { type: 'image/png' });

        var fileName = UTF8ToString(fileNamePtr);
        var url = URL.createObjectURL(blob);
        var a = document.createElement('a');
        a.href = url;
        a.download = fileName;
        a.click();

        URL.revokeObjectURL(url);
    }
});
