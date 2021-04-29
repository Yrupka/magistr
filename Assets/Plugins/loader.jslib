mergeInto(LibraryManager.library, {

  Get_file_data: function () {
    var allText = "";
    var rawFile = new XMLHttpRequest();
    rawFile.open("GET", "profile.json", false);
    rawFile.onreadystatechange = function ()
    {
        if(rawFile.readyState === 4)
        {
            if(rawFile.status === 200 || rawFile.status == 0)
                allText = rawFile.responseText;
            else
                allText = "error";
            if (allText == null)
                allText = "error";
        }
    }
    rawFile.send(null);
    var bufferSize = lengthBytesUTF8(allText) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(allText, buffer, bufferSize);
    return buffer;
  },

});