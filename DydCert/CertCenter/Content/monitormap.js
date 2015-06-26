
var monitormap = (function () {
    var url_init = '/monitor/InitMonitor';
    var url_data = '/monitor/Getdata';
    var _eleid = 'monitor_drawpanel';

    function writelog(txt) {
        var e = $$("log");
        e.innerHTML = e.innerHTML + txt + "<br />";
        e.scrollTop = e.scrollHeight;
        e.scrollIntoView(true);
    }

    function $$(id) {
        if ((id + "").charAt(0) == '.') {
            return document.getElementsByClassName((id + "").substr(1));
        }
        return document.getElementById(id);
    }
    var loophandler = -1;
    function _start() {
        if (loophandler > 0)
            _stop();
        drawmap.clear();
        getdata(function (d) {
            if (d.code > 0) {
                var wraptime = d.data.wraptime > 100 ? d.data.wraptime : 500;
                loophandler = setInterval(drawmap.draw, wraptime);
            }
        });
    }

    function getdata(callback) {
        inajax(url_data, "post", null, callback);
    }

    function _stop() {
        if (loophandler > 0) {
            clearInterval(loophandler);
            loophandler = -1;
            return true;
        }
        return false;
    }
    function _init(eleid, timespan) {
        _eleid = eleid;
        inajax(url_init, "post", { miseconds: timespan }, function (x) { });
    }

    var drawmap = (function () {
        var mappoints = new Array();
        var nowposition = 0;
        var width = 500;
        var height = 400;
        function getdrawpanel(k) {
            var dd = $$(_eleid + "_dmap_" + k);
            if (dd) {
                return dd;
            } else {
                return createele(k);
            }
        }

        function drawact(data) {
            for (var i = 0; i < data.length; i++) {
                var existpoint = false;
                for (var k = 0; k < mappoints.length; k++) {
                    if (mappoints[k].url == data[i].url) {
                        existpoint = true;
                        break;
                    }
                }
                if (!existpoint) {
                    mappoints.push({
                        url: data[i].url,
                        count: 0,
                        point: 0,
                        pnlindex: mappoints.length + 1,
                        color: getrandomcolor()
                    });
                }
            }
            var x_old = nowposition;
            nowposition += 20;
            var x_new = nowposition;
            if (x_new > width)
                _clear();
            for (var t = 0; t < mappoints.length; t++) {
                var mapele = getdrawpanel(mappoints[t].pnlindex);
                var oldp = mappoints[t];
                var newp = getdatapoint(oldp.url, data);
                var y_old = height - oldp.point;
                var y_new = height - newp.point;
                var l_color = mappoints[t].color;
                var cans = mapele.getContext('2d');

                cans.moveTo(x_old, y_old);//第一个起点
                cans.lineTo(x_new, y_new);//第二个点
                cans.lineWidth = 3;
                cans.strokeStyle = l_color;
                cans.stroke();
                console.log(newp);
                //更新点
                writelog(oldp.url + "={" + x_old + "," + y_old + "},   {" + x_new + "," + y_new + "},color:" + l_color);
                oldp.point = newp.point;
                oldp.count = newp.count;
            }

        }
        function getrandomcolor() {
            var r = (parseInt(Math.random() * 10000) % 255).toString(16);
            var g = (parseInt(Math.random() * 10000) % 255).toString(16);
            var b = (parseInt(Math.random() * 10000) % 255).toString(16);
            return "#" + r + g + b;
        }

        function getdatapoint(url, data) {
            if (data) {
                for (var i = 0; i < data.length; i++) {
                    if (data[i].url == url)
                        return {
                            url: data[i].url,
                            point: data[i].lastcount,
                            count: data[i].lastcount,
                            color: ""
                        };
                }
            }
            return {
                url: url,
                count: 0,
                point: 0,
                color: ""
            };
        }

        function createele(k) {
            var e = document.createElement("canvas");
            e.setAttribute("id", _eleid + "_dmap_" + k);
            e.setAttribute("width", width + "px");
            e.setAttribute("height", height + "px");
            var xd = $$(_eleid).appendChild(e);
            return xd;
        }
        function _clear() {
            //initdraw();
            //if (mappoints) {
            //    for (var i = 0; i < mappoints.length; i++) {
            //        var mapele = getdrawpanel(mappoints[i].pnlindex);
            //        var cans = mapele.getContext('2d');
            //        cans.fillStyle = 'white';
            //        cans.fillRect(0, 0, width, height);
            //    }
            //}
            // var mapele = undefined;
            //mappoints = new Array();
            var e = $$(_eleid);
            for (var x = 0; x < e.childNodes.length; x++) {
                e.removeChild(e.childNodes[x]);
            }
            e.innerHTML = "";
            nowposition = 0;
        }
        return {
            draw: function () {
                getdata(function (data) {
                    if (data.code > 0) {
                        drawact(data.data.maps);
                    }

                });
            },
            clear: function () { _clear(); }
        };
    })();

    var inajax = function (url, method, data, fun) {
        var XMLHttpReq;
        function createXMLHttpRequest() {
            try {
                XMLHttpReq = new ActiveXObject("Msxml2.XMLHTTP");//IE高版本创建XMLHTTP
            }
            catch (E) {
                try {
                    XMLHttpReq = new ActiveXObject("Microsoft.XMLHTTP");//IE低版本创建XMLHTTP
                }
                catch (E) {
                    XMLHttpReq = new XMLHttpRequest();//兼容非IE浏览器，直接创建XMLHTTP对象
                }
            }

        }
        function sendAjaxRequest() {
            createXMLHttpRequest();                                //创建XMLHttpRequest对象
            XMLHttpReq.open(method, url, true);
            XMLHttpReq.onreadystatechange = processResponse; //指定响应函数
            var sendcontend = "";
            if (data) {
                for (var kvp in data) {
                    if (sendcontend != "") {
                        sendcontend += "&";
                    }
                    sendcontend += kvp + "=" + encodeURI(data[kvp]);
                }
            }
            XMLHttpReq.send(sendcontend);
        }
        //回调函数
        function processResponse() {
            if (XMLHttpReq.readyState == 4) {
                if (XMLHttpReq.status == 200) {
                    var text = XMLHttpReq.responseText;
                    if (fun) {
                        fun(JSON.parse(text));
                    }
                }
            }
        }
        sendAjaxRequest();
    };

    return {
        start: _start,
        stop: _stop,
        init: _init
    };
})();