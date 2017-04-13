//检测pc端，移动端
var sUserAgent = navigator.userAgent.toLowerCase();
var bIsIpad = sUserAgent.match(/ipad/i) == "ipad";
var bIsIphoneOs = sUserAgent.match(/iphone os/i) == "iphone os";
var bIsMidp = sUserAgent.match(/midp/i) == "midp";
var bIsUc7 = sUserAgent.match(/rv:1.2.3.4/i) == "rv:1.2.3.4";
var bIsUc = sUserAgent.match(/ucweb/i) == "ucweb";
var bIsAndroid = sUserAgent.match(/android/i) == "android";
var bIsCE = sUserAgent.match(/windows ce/i) == "windows ce";
var bIsWM = sUserAgent.match(/windows mobile/i) == "windows mobile";
if (bIsIpad || bIsIphoneOs || bIsMidp || bIsUc7 || bIsUc || bIsAndroid || bIsCE || bIsWM) {
    window.location.href = "http://mjy.1fix.cn";
} else {
    window.location.href = "http://jy.1fix.cn";
}

//写cookies
function setCookie(name, value) {
    var Days = 30;
    var exp = new Date();
    exp.setTime(exp.getTime() + Days * 24 * 60 * 60 * 1000);
    document.cookie = name + "=" + escape(value) + ";expires=" + exp.toGMTString();
    var strsec = 0.5 * 60 * 60 * 1000; //30分钟
    var exp = new Date();
    exp.setTime(exp.getTime() + strsec * 1);
    document.cookie = name + "=" + escape(value) + ";expires=" + exp.toGMTString();
}
//读取cookies
function getCookie(name) {
    var arr, reg = new RegExp("(^| )" + name + "=([^;]*)(;|$)");

    if (arr = document.cookie.match(reg))

        return (arr[2]);
    else
        return null;
}
//删除cookies
function delCookie(name) {
    var exp = new Date();
    exp.setTime(exp.getTime() - 1);
    var cval = getCookie(name);
    if (cval != null)
        document.cookie = name + "=" + cval + ";expires=" + exp.toGMTString();
}





//除法函数
function accDiv(arg1, arg2) {
    var t1 = 0, t2 = 0, r1, r2, n;
    try {
        t1 = arg1.toString().split(".")[1].length;
    }
    catch (e)
    { t1 = 0; }
    try {
        t2 = arg2.toString().split(".")[1].length;
    }
    catch (e)
    { t2 = 0; }
    with (Math) {
        r1 = Number(arg1.toString().replace(".", ""));
        r2 = Number(arg2.toString().replace(".", ""));
        n = Math.max(t1, t2);
        return (r1 / r2) * pow(10, t2 - t1);
    }
}

//乘法函数
function accMul(arg1, arg2) {
    var t1 = 0, t2 = 0, r1, r2;
    try {
        t1 = arg1.toString().split(".")[1].length;
    }
    catch (e)
    { t1 = 0; }
    try {
        t2 = arg2.toString().split(".")[1].length;
    }
    catch (e)
    { t2 = 0; }
    with (Math) {
        r1 = Number(arg1.toString().replace(".", ""));
        r2 = Number(arg2.toString().replace(".", ""));
        return (r1 * r2) / pow(10, t2 + t1);
    }
}

//加法函数
function accAdd(arg1, arg2) {
    var t1 = 0, t2 = 0, m;
    try {
        t1 = arg1.toString().split(".")[1].length;
    }
    catch (e)
    { t1 = 0; }
    try {
        t2 = arg2.toString().split(".")[1].length;
    }
    catch (e)
    { t2 = 0; }
    with (Math) {
        m = Math.pow(10, Math.max(t1, t2));
        return (arg1 * m + arg2 * m) / m;
    }
}

//减法函数
function accSubtr(arg1, arg2) {
    var t1 = 0, t2 = 0, m, n;
    try {
        t1 = arg1.toString().split(".")[1].length;
    }
    catch (e)
    { t1 = 0; }
    try {
        t2 = arg2.toString().split(".")[1].length;
    }
    catch (e)
    { t2 = 0; }
    with (Math) {
        //动态控制精度长度
        n = Math.max(t1, t2);
        m = Math.pow(10, n);
        //return (arg1  * m - arg2 * m) / m;
        return ((arg1 * m - arg2 * m) / m).toFixed(n);
    }
}

