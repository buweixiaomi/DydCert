
Function.prototype.method = function (name, fn) {
    this.prototype[name] = fn;
};

var xpdialog = function () {
    this.url = "";
    this.data = "";
    this.content = "";
    this.buttons = undefined;
    this.title = "dialog";
    this.dwidth = -1;
    this.cheight = -1;
};

xpdialog.method("show", function () {
    var thisinstance = this;
    if (this.url && this.url != "") {
        $.ajax({
            url: this.url,
            data: this.data,
            async: false,
            success: function (rdata) {
                thisinstance.content = rdata;
            },
            error: function () {
                thisinstance.content = "请求url:" + this.url + "出错";
            }
        });
    }
    var tempclosebuton = $("<div class='xpdialog_close_button'>X</div>").click(this.close);

    var titlediv = $("<div class='xpdialog_title'>" + this.title + "</div>");
    var tempcontent;
    if (this.cheight < 0) {
        tempcontent = $("<div class='xpdialog_content' >" + this.content + "</div>");
    } else {
        tempcontent = $("<div class='xpdialog_content' style=\"height:" + this.cheight + "px;overflow-y:auto;\">" + this.content + "</div>");
    }
    var tempbutton = $("<div class='xpdialog_opbuttons'></div>");
    if (this.buttons) {
        for (var a in this.buttons) {
            var nbtn = $("<input type='button' value='" + this.buttons[a].title + "' /> ");
            if (this.buttons[a].handler) {
                $(nbtn).click(this.buttons[a].handler);
            }
            $(tempbutton).append(nbtn);
        }
    }
    $("body").append("<div class='xpdialog_panel' style='position:fixed; left:0;top:0;width:100%; margin:0; padding:0; height:100%; background-color:black;z-index:998;filter:alpha(opacity=50); -moz-opacity:0.5; -khtml-opacity: 0.5; opacity: 0.5;'></div>");
    var disatempstring;
    if (this.dwidth < 0) {
        disatempstring = "<div class='xpdialog'></div>";
    } else {
        disatempstring = "<div class='xpdialog' style=\"width:" + this.dwidth + "px;\"></div>";
    }
    var xpdialogstr = $(disatempstring).append(tempclosebuton).append(titlediv).append(tempcontent).append(tempbutton);//  = document.createElement("div");
    $("body").append(xpdialogstr);
    var mleft = $(window).width()/ 2 - $(".xpdialog").width() / 2;
    var mtop = $(window).height() / 2 - $(".xpdialog").height() / 2 - 20;
    $(".xpdialog").css({ position: 'fixed', left: mleft + 'px', top: mtop + 'px', display: 'block','z-index':999 });
    $(window).resize(function () {
        if ($(".xpdialog").length != 0) {
            var mleft =  $(window).width() / 2 - $(".xpdialog").width() / 2;
            var mtop = $(window).height() / 2 - $(".xpdialog").height() / 2 - 20;
            $(".xpdialog").css({position:'fixed',left: mleft + 'px', top: mtop + 'px', display: 'block' ,'z-index':999});
        }
    });
});
xpdialog.method("close", function () {
    $(".xpdialog").remove();
    $(".xpdialog_panel").remove();
});