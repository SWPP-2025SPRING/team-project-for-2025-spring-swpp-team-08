mergeInto(LibraryManager.library, {
  SyncFileSystem: function() {
    console.log('SyncFileSystem called');
    FS.syncfs(false, function (err) {
        console.log('Error: syncfs failed!');
    });
  }
});