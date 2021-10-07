

function _p31text_edi(txt, p31id) {
    var s = $(txt).val();
    
    $.post(_ep("/p31oper/UpdateText"), { p31id: p31id, s: s }, function (data) {
        if (data.flag === 0) {
            _notify_message(data.flag + ", message: " + data.message);
        } else {
            _notify_message("Úpravy uloženy.", "info");
        }


    });
}