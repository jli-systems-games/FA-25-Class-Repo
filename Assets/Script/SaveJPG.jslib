mergeInto(LibraryManager.library, {
  SaveJPG: function (base64Ptr, namePtr) {
    var base64 = UTF8ToString(base64Ptr);
    var name = UTF8ToString(namePtr);

    var link = document.createElement('a');
    link.href = "data:image/jpeg;base64," + base64;
    link.download = name;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
  }
});