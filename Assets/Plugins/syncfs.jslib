mergeInto(LibraryManager.library, {
  SyncFileSystem: function() {
    FS.syncfs(false, function(err) {
      // Optionally handle error
    });
  }
});