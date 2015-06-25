

function refillgrades(apptype) {
    $.ajax({
        url: '/Appgrade/UseApptypeGetGrades',
        type: 'post',
        data: { apptype: apptype },
        success: function (data) {
            if (data.code > 0) {
    $("#appgradeno").html("");
                for (var i = 0; i < data.response.length; i++) {
                    $("#appgradeno").append("<option value=\"" + data.response[i].appgradeno + "\">" + data.response[i].appgradeno + "-" + data.response[i].appgradename + "</option>");
                }
            }
        },
        error: function (ex) {
            alert("你的网络开小差了，请重试。");
        }
    });
}

function refillcategory(apptype) {
    $.ajax({
        url: '/apicategory/UseApptypeGetcategories',
        type: 'post',
        data: { apptype: apptype },
        success: function (data) {
            if (data.code > 0) {
    $("#categoryid").html("");
                for (var i = 0; i < data.response.length; i++) {
                    $("#categoryid").append("<option value=\"" + data.response[i].apicategoryid + "\">" + data.response[i].apicategoryid + "-" + data.response[i].categorytitle + "</option>");
                }
            }
        },
        error: function (ex) {
            alert("你的网络开小差了，请重试。");
        }
    });
}