
var xpinfodialog = (function () {
    
    //return function () {
    return this;
    //};
})();

xpinfodialog.show = function (content, time) {
    $('.xpinfodialog').remove();
    var a;
    var condiv = "<div class='xpinfodialog' style='position:fixed; left:40%; top:0; background-color:#2a8dbb;color:white;font-size:16px; padding:4px 10px; -moz-border-radius: 4px; -webkit-border-radius: 4px; border-radius:4px;  font-family:黑体;'>" + content + "</div>";
    if (time == undefined)
        time = 3000;
    $(condiv).hover(function () {
        clearTimeout(a);
    }, function () {
        a = setTimeout(function () {
            $('.xpinfodialog').hide(200);

            setTimeout(function () {
                $('.xpinfodialog').remove();
            }, 200);
        }, time);
    }).appendTo("body");
    var _left = window.innerWidth / 2 - $(".xpinfodialog").width() / 2;
    $(".xpinfodialog").css({
        left:_left+'px'
    });
    var a = setTimeout(function () {
        $('.xpinfodialog').hide(200);
        setTimeout(function () { 
        $('.xpinfodialog').remove();}, 200);
    }, time);
};