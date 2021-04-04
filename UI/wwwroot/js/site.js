//na úvod detekce mobilního zařízení přes _device
var _device = {
    isMobile: false,
    type: "Desktop",
    availHeight: screen.availHeight,
    availWidth: screen.availWidth,
    innerWidth: window.innerWidth,
    innerHeight: window.innerHeight    
}
if (screen.availHeight > screen.availWidth || screen.width < 800 || screen.height < 600) {   //mobilní zařízení výšku vyšší než šířku
    _device.isMobile = true;
    _device.type = "Phone";

}

function _ep(url)   //vrací relativní cestu z url výrazu
{    
    if (url.indexOf("//") > 0) {
        return url;
    }
    if (_relpath==="/" && url.substring(0, 1) === "/") {
        return url;
    }
    if (url.substring(0, 1) === "/") {
        url = url.substring(1, url.length - 1);
    }
    return _relpath + url;
}

function _format_number(val) {
    if (val === null) {
        return ("");
    }

    var s = val.toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, '$1,');
    return (s);
}
function _format_number_int(val) {
    if (val === null) {
        return ("");
    }
    var s = val.toFixed(0).replace(/(\d)(?=(\d{3})+\.)/g, '$1,');
    return (s);
}

function _format_date(d, is_include_time) {

    var month = '' + (d.getMonth() + 1);
    var day = '' + d.getDate();
    var year = d.getFullYear();
    var hour = d.getHours();
    var minute = d.getMinutes();

    if (is_include_time === true) {
        return (day + "." + month + "." + year + " " + hour + ":" + minute);
    } else {
        return (day + "." + month + "." + year);
    }

}

function _edit(controller, pid, header) {
    var url = "";
    var winflag = 1;
    switch (controller) {
        case "x40":
            url = "/Mail/Record?pid=" + pid;
            break;

        case "j90":
        case "j92":        
            _notify_message("Pro tento záznam neexistuje stránka detailu.", "info");
            return;
        default:
            url = "/" + controller + "/record?pid=" + pid;
            break;
    }
    if (controller === "p51") {
        winflag = 2;
    }
   
    _window_open(_ep(url), winflag, header);

}

function _clone(controller, pid, header) {
    _window_open(_ep("/" + controller + "/record?isclone=true&pid=" + pid), 1, header);

}

function _edit_code(prefix, pid) {
    _window_open(_ep("/Record/RecordCode?prefix=" + prefix + "&pid=" + pid, 1));
}

function _get_request_param(name) {
    var results = new RegExp("[\?&]" + name + "=([^&#]*)").exec(window.location.href);
    return results ? decodeURIComponent(results[1].replace(/\+/g, '%20')) : null;
}


function _notify_message(strMessage, strTemplate = "error", milisecs = "3000") {
    if (document.getElementById("notify_container")) {
        //notify div na stránce již existuje          
    } else {
        var el = document.createElement("DIV");
        $(el).css("position", "absolute");
        $(el).css("top", "0px");
        if (screen.availWidth > 500) $(el).css("left", window.innerWidth - 500);

        el.id = "notify_container";
        document.body.appendChild(el);
    }
    if (strTemplate) {
        if (strTemplate === "") strTemplate = "info";
    } else {
        strTemplate = "info";
    }


    var img = "info", intTimeoutSeconds = 5000;
    if (strMessage.length > 250) intTimeoutSeconds = 10000;

    if (strTemplate === "error") {
        img = "exclamation-triangle";
        strTemplate = "danger";
    }
    if (strTemplate === "warning") img = "exclamation";
    if (strTemplate === "success") img = "thumbs-up";
    var toast_id = "toast" + parseInt(100000 * Math.random());

    var node = document.createElement("DIV");
    node.id = "box" + parseInt(100000 * Math.random());
    var w = "400px";
    if (screen.availWidth < 400) w = "95%";

    var s = "<div id='" + toast_id + "' class='toast' data-autohide='true' data-delay='" + milisecs + "' data-animation='true' style='margin-top:10px;min-width:" + w + ";'>";
    s = s + "<div class='toast-header text-dark bg-" + strTemplate + "'><i class='fas fa-" + img + "'></i>";
    //s = s + "<strong class='mr-auto' style='color:black;'>Toast Header</strong>";
    s = s + "<div style='width:90%;'> " + strTemplate + "</div><button type='button' class='ml-2 mb-1 close' data-dismiss='toast'>&times;</button>";
    s = s + "</div>";
    s = s + "<div class='toast-body' style='font-size:130%;'>";
    s = s + strMessage;
    s = s + "</div>";
    s = s + "</div>";


    $(node).html(s);
    document.getElementById("notify_container").appendChild(node);

    if (typeof is_permanent !== "undefined") {
        if (is_permanent === true) return;
    }

    $("#" + toast_id).toast("show");



}


//vyvolání kontextového menu
function _cm(e, entity, pid, flag) {

    var ctl = e.target;

    var w = $(window).width();
    var pos_left = e.clientX + $(window).scrollLeft();

    var cssname = "cm_left2right";
    if (pos_left + 300 >= w) cssname = "cm_right2left";
    if (_device.type === "Phone") {
        cssname = "cm_mobile";

    }
    var menuid = "cm_" + entity + "_" + pid;

    if (!document.getElementById(menuid)) {
        //div na stránce neště existuje
        var el = document.createElement("DIV");
        el.id = menuid;
        el.className = cssname;
        el.style.display = "none";
        document.body.appendChild(el);
    }


    if (ctl.getAttribute("menu_je_inicializovano") === "1") {
        return; // kontextové menu bylo již u tohoto elementu inicializováno - není třeba to dělat znovu.
    }

    var menuLoadByServer = true;

    if (document.getElementById("divContextMenuStatic")) {
        var data = $("#divContextMenuStatic").html();   //na stránce se nachází preferované UL statického menu v divu id=divContextMenuStatic -> není třeba ho volat ze serveru
        data = data.replace(/#pid#/g, pid);  //místo #pid# replace pravé pid hodnoty
        $("#" + menuid).html(data);

        menuLoadByServer = false;

    } else {
        //načíst menu později dynamicky ze serveru   
        $("#" + menuid).html("Loading...");
        menuLoadByServer = true;

    }

    
    $(ctl).contextMenu({
        menuSelector: "#" + menuid,
        menuClicker: ctl,
        menuEntity: entity,
        menuPid: pid,
        menuFlag: flag,
        menuLoadByServer: menuLoadByServer

    });
    ctl.setAttribute("menu_je_inicializovano", "1");




}

function _update_user_ping() {

    var devicetype = "Desktop";
    if (screen.availHeight > screen.availWidth || screen.width < 800 || screen.height < 600) {   //mobilní zařízení výšku vyšší než šířku
        devicetype = "Phone";
    }

    var log = {
        j92BrowserUserAgent: navigator.userAgent,
        j92BrowserAvailWidth: screen.availWidth,
        j92BrowserAvailHeight: screen.availHeight,
        j92BrowserInnerWidth: window.innerWidth,
        j92BrowserInnerHeight: window.innerHeight,
        j92BrowserDeviceType: devicetype,
        j92RequestURL: location.href.replace(location.host, "").replace(location.protocol, "").replace("///", "")
    }


    $.post("/Home/UpdateCurrentUserPing", { c: log }, function (data) {
        //ping aktualizován
    });

}


function _mainmenu_highlight_current(linkID) {
    if (document.getElementById(linkID)) {
        $("#" + linkID).addClass("active");  //označit odkaz v hlavním menu
    }
}

function _save_as_home_page() {
    var url = location.href.replace(location.host, "").replace(location.protocol, "").replace("///", "");
    url = _removeUrlParam("go2pid", url);
    $.post("/Home/SaveCurrentUserHomePage", { homepageurl: url }, function (data) {
        if (data.flag === 1) {
            location.replace("/");
            //_notify_message("Domovská stránka uložena: <hr>"+url, "info");
        }

    });
}
function _clear_home_page() {
    $.post("/Home/SaveCurrentUserHomePage", { homepageurl: "" }, function (data) {
        if (data.flag === 1) {

            location.replace("/Dashboard/Index");
        }

    });
}

function _save_fontsize(fontsize) {
    $.post("/Home/SaveCurrentUserFontSize", { fontsize: fontsize }, function (data) {
        location.replace(window.location.href);
    });

}

function _save_langindex_menu(langindex) {
    $.post("/Home/SaveCurrentUserLangIndex", { langindex: langindex }, function (data) {
        location.replace(window.location.href);
    });
}



function _removeUrlParam(key, sourceURL) {
    var rtn = sourceURL.split("?")[0],
        param,
        params_arr = [],
        queryString = (sourceURL.indexOf("?") !== -1) ? sourceURL.split("?")[1] : "";
    if (queryString !== "") {
        params_arr = queryString.split("&");
        for (var i = params_arr.length - 1; i >= 0; i -= 1) {
            param = params_arr[i].split("=")[0];
            if (param === key) {
                params_arr.splice(i, 1);
            }
        }
        if (params_arr.length > 0) {
            rtn = rtn + "?" + params_arr.join("&");
        }

    }
    return rtn;
}


function _resize_textareas() {
    $("textarea").each(function () {
        this.style.height = "auto";
        this.style.height = (this.scrollHeight) + "px";
        
        $(this).on("input", function () {
            this.style.height = "auto";
            this.style.height = (this.scrollHeight) + "px";
        });
    });
}



////z elementu se stane draggable:
function _make_element_draggable(elmnt, inner_elmnt_4hide) {
    var pos1 = 0, pos2 = 0, pos3 = 0, pos4 = 0;
    if (document.getElementById(elmnt.id + "header")) {
        /* if present, the header is where you move the DIV from:*/
        document.getElementById(elmnt.id + "header").onmousedown = dragMouseDown;
    } else {
        /* otherwise, move the DIV from anywhere inside the DIV:*/
        elmnt.onmousedown = dragMouseDown;
    }

    function dragMouseDown(e) {


        e = e || window.event;
        e.preventDefault();
        // get the mouse cursor position at startup:
        pos3 = e.clientX;
        pos4 = e.clientY;
        document.onmouseup = closeDragElement;
        // call a function whenever the cursor moves:
        document.onmousemove = elementDrag;
    }

    function elementDrag(e) {
        inner_elmnt_4hide.style.display = "none";
        e = e || window.event;
        e.preventDefault();
        // calculate the new cursor position:
        pos1 = pos3 - e.clientX;
        pos2 = pos4 - e.clientY;
        pos3 = e.clientX;
        pos4 = e.clientY;
        // set the element's new position:
        elmnt.style.top = (elmnt.offsetTop - pos2) + "px";
        elmnt.style.left = (elmnt.offsetLeft - pos1) + "px";
    }

    function closeDragElement() {
        /* stop moving when mouse button is released:*/
        inner_elmnt_4hide.style.display = "block";
        $("#inner_elmnt_4hide").attr("disabled", "");
        document.onmouseup = null;
        document.onmousemove = null;
    }
}


//inicializovat na formuláři všechny qtips
function _init_qtip_onpage() {
    //qtip:

    var iframeWidth = 800;
    var maxwidth = $(window).innerWidth();
    if (maxwidth < 800) {
        iframeWidth = maxwidth;
    }

    if (window !== top && _get_request_param("hover_by_reczoom") === "1") {   //voláno uvnitř qtip iframe: zde už reczoom schovat
        $("a.reczoom").each(function () {
            $(this).css("display", "none");
            $(this).removeClass("reczoom");
        });
        return;
    }

    $("a.reczoom").each(function () {

        var mywidth = iframeWidth;
        var $this = $(this);
        var myurl = $this.attr("data-rel");
        var mytitle = $this.attr("data-title");
        if (mytitle === null || mytitle === "") {
            mytitle = "Detail";
        }
        if ($this.attr("data-maxwidth") === "1") {
            mywidth = $(window).innerWidth() - 10;     //okno má mít maximální šířku            
        }




        $this.qtip({
            content: {
                text: "<iframe id='fraRecZoom' framemargin='0' style='height:270px;width:100%;' src='" + myurl + "'></iframe>",
                title: {
                    text: mytitle
                },

            },
            position: {
                my: "top center",  // Position my top left...
                at: "bottom center", // at the bottom right of...
                viewport: $(window),
                adjust: {
                    method: "shift"
                }
            },

            hide: {
                fixed: true,
                delay: 100
            },
            style: {
                classes: "qtip-tipped",
                width: mywidth,
                height: 300

            }
        });
    });
}


//upozornění uživatele na editaci prvku na formuláři
function _toolbar_warn2save_changes(message) {
    if (typeof message === "undefined") {
        message = "Změny potvrďte tlačítkem [Uložit změny]."
    }
    if ($("#toolbar_changeinfo").length) {
        $("#toolbar_changeinfo").text(message);
    }

}

//spustit hardrefresh na volající stránkce

function _reload_layout_and_close(pid, flag) {

    if (window !== top) {
        window.parent.hardrefresh(pid, flag);
        window.parent._window_close();
    } else {
        hardrefresh(pid, flag);
    }
}


//vyvolání zoom info okna
function _zoom(e, entity, pid, dialog_width, header, url) {     //wtype: small (600px) nebo big (1050px), výchozí je small
    var ctl = e.target;
    //var ofs = $(ctl).offset();
    var maxwidth = $(window).innerWidth();
    var maxheight = $(window).innerHeight();

    if (maxwidth > 1000) maxwidth = 1000;

    var w = 600;
    var h = 600;

    if (typeof dialog_width !== "undefined") {
        w = dialog_width;
    }
    if (w > maxwidth) {
        w = maxwidth - 10;
    }
    if (h > maxheight) {
        h = maxheight - 10;
    }
    if (typeof url === "undefined") {
        url = "/" + entity + "/Info?pid=" + pid;
    }
    if (typeof header === "undefined") {
        header = "INFO";
    }

    var menuid = "zoom_okno";
    //if (pos_left + w >= maxwidth) menuid = "cm_right2left";

    if (!document.getElementById(menuid)) {
        //div na stránce neště existuje
        var el = document.createElement("DIV");
        el.id = menuid;
        el.style.display = "none";
        document.body.appendChild(el);
    }

    $("#" + menuid).width(w);
    $("#" + menuid).height(h);


    var s = "<div id='divZoomContainer' style='border:solid 1px silver;width:" + w + "px;' orig_w='" + w + "' orig_h='" + h + "'>";
    s += "<div id='divZoomHeader' style='cursor: move;height:30px;background-color:lightsteelblue;padding:3px;' ondblclick='_zoom_toggle()'>" + header + "</div>";
    s += "<div id='divZoomFrame'>";
    s += "<iframe id='frazoom' src = '" + url + "' style = 'width:100%;height: " + (h - 31) + "px;' frameborder=0></iframe >";
    s += "</div>";
    s += "</div>";

    $("#" + menuid).html(s);

    _make_element_draggable(document.getElementById("zoom_okno"), document.getElementById("divZoomFrame")); //předávání nefunguje přes jquery

    if (ctl.getAttribute("menu_je_inicializovano") === "1") {

        return; // kontextové menu bylo již u tohoto elementu inicializováno - není třeba to dělat znovu.
    }



    $(ctl).contextMenu({
        menuSelector: "#" + menuid,
        menuClicker: ctl,
        menuLoadByServer: false

    });

    ctl.setAttribute("menu_je_inicializovano", "1");
}

function _zoom_toggle() {
    var okno = $("#divZoomContainer");
    var offset = $(okno).offset();

    var w = $(window).width() - offset.left - 10;
    var h = $(window).height() - offset.top - 10;

    if ($(window).width() - offset.left - $(okno).width() < 30) {
        w = $("#divZoomContainer").attr("orig_w");
        h = $("#divZoomContainer").attr("orig_h");
    }

    $(okno).width(w);
    $(okno).height(h);
    $("#divZoomFrame").height(h - 31);
    $("#frazoom").height(h - 31);

}

function _helppage() {
    var s = document.title.replace(" - MARKTIME", "");
    var viewurl = window.location.pathname.split('?')[0];
    try {
        _window_open("/x51/Index?viewurl=" + viewurl, 1, "Help");
    } catch (err) {
        window.open("/x51/Index?viewurl=" + viewurl + "&pagetitle=" + s, "_blank");     //pokud na stránce není definice metody _window_open (např. reporting)
    }    
}

function _helppage_layout() {
    var s = document.title.replace(" - MARKTIME", "");

    var viewurl = window.location.pathname.split('?')[0] + window.location.search;
    _window_open("/x51/Index?viewurl=" + viewurl, 1, "Help");    
}


function _resize_iframe_onpage(iframe_id) {
    var offset = $("#" + iframe_id).offset();
    var remain_height = _device.innerHeight - offset.top;
    remain_height = parseInt(remain_height) - 20;
    if (_device.type === "Phone") {
        h_vertical = 400;
    }
    $("#" + iframe_id).css("height", remain_height + "px");
}