mergeInto(LibraryManager.library, {

GetUTCTicks: function () {
    return 621355968000000000 + new Date().getTime() * 10000;
},

FlushFileSystem: function() {
	FS.syncfs(false, function (err) {
		if (err) {
			console.log('[ACTk]: error flushing file system ' + err);
		}
	});
},

});