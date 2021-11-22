

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

function _p31_create(p34id, p31date) {
    var url = "/p31/Record?pid=0";
    if (p34id !== undefined && p34id !=="0") {
        url = url + "&p34id=" + p34id;
    }
    if (p31date !== undefined) {
        url = url + "&p31date=" + p31date;
    }
    _window_open(url);
}




