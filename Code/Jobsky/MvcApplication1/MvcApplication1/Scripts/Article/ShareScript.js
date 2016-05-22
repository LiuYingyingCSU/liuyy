/*这个js管理着三个编辑文章页面，请谨慎操作*/

var demandNumber = 2; //增加需求逻辑控制量

//添加需求
$(function () {
    $("#btnAdd").click(function () {
        var arraydemandhtml = ['<div class="demand"><div class="editor-label"><label><span class="red-star">*</span>职位名称:</label><input id="PositionName" name="PositionName" type="text" required="required" /></div><div class="editor-label"><label><span class="red-star">*</span>教育水平:</label><select id="EducationalLevel" name="EducationalLevel"><option value="本科生">本科生</option><option value="硕士研究生">硕士研究生</option><option value="博士研究生">博士研究生</option><option value="本科及其以上">本科及其以上</option></select></div><div class="editor-label"><label><span class="red-star">*</span>专业:</label><div class="topnav" style="display:inline;"><a id="selectmajor' + demandNumber + '" href="javascript:void(0);" class="as"><span>请选择</span></a><input id="LimitMajor1" type="checkbox" value="不限制" onclick="LimitMajorCheck(this)" style="margin-left:20px;" />不限制<br /></div><textarea id="Major' + demandNumber + '" class="Major"  name="Major" required="required" readonly="readonly" style="height:100px; margin-left:12%;margin-top:10px;"></textarea></div><div class="editor-label"><label><span class="red-star">*</span>需求数量:</label><input id="DemandNum" name="DemandNum" type="number" required="required" onkeyup="value = value.replace(\'^[0-9]*[1-9][0-9]*$\', \'\')" placeholder="请填正整数" /></div><div class="editor-label"><label>&nbsp;&nbsp;&nbsp;职位描述:</label><textarea id="textarea3" name="PositionDec" style="height:100px;"></textarea></div><input id="btnDel" type="button" value="删除需求" /></div>'];
        var arrayselectdemandhtml = ['<div id="m' + demandNumber + '" class="xmenu" style="display: none;"> <div class="select-info"><label class="top-label">已选职位：</label><ul></ul><a name="menu-confirm" href="javascript:void(0);" class="a-btn"><span class="a-btn-text">确定</span></a></div><dl><dt>信息科学与工程学院</dt><dd style="display: none;"><ul><li rel="自动化">自动化</li><li rel="电气工程及其自动化">电气工程及其自动化</li><li rel="测控技术与仪器">测控技术与仪器</li><li rel="电子信息工程">电子信息工程</li><li rel="通信工程">通信工程</li><li rel="计算机科学与技术">计算机科学与技术</li><li rel="信息安全">信息安全</li><li rel="物联网工程">物联网工程</li><li rel="智能科学与技术">智能科学与技术</li></ul></dd><dt>数学与统计学院</dt><dd style="display: none;"><ul><li rel="数学与应用数学">数学与应用数学</li><li rel="信息与计算科学">信息与计算科学</li><li rel="统计学">统计学</li></ul></dd><dt>文学与新闻传播学院</dt><dd style="display: none;"><ul><li rel="汉语言文学">汉语言文学</li><li rel="广播电视新闻学">广播电视新闻学</li></ul></dd><dt>法学院</dt><dd style="display: none;"><ul><li rel="法学">法学</li></ul></dd><dt>粉末冶金研究院</dt><dd style="display: none;"><ul><li rel="材料化学">材料化学</li><li rel="粉体材料科学与工程">粉体材料科学与工程</li></ul></dd><dt>冶金与环境学院</dt><dd style="display: none;"><ul><li rel="冶金工程">冶金工程</li><li rel="环境工程">环境工程</li><li rel="新能源材料与器件">新能源材料与器件</li></ul></dd><dt>机电工程学院</dt><dd style="display: none;"><ul><li rel="机械设计制造及其自动化">机械设计制造及其自动化</li><li rel="车辆工程">车辆工程</li><li rel="微电子制造工程">微电子制造工程</li></ul></dd><dt>外国语学院</dt><dd style="display: none;"><ul><li rel="英语">英语</li><li rel="法语">法语</li><li rel="日语">日语</li><li rel="西班牙语">西班牙语</li></ul></dd><dt>公共管理学院</dt><dd style="display: none;"><ul><li rel="行政管理">行政管理 </li><li rel="社会学">社会学</li></ul></dd><dt>资源与安全工程学院</dt><dd style="display: none;"><ul><li rel="采矿工程">采矿工程</li><li rel="安全工程">安全工程</li><li rel="城市地下空间工程">城市地下空间工程</li></ul></dd><dt>地球科学与信息物理学院</dt><dd style="display: none;"><ul><li rel="地质工程">地质工程</li><li rel="地球信息科学与技术">地球信息科学与技术</li><li rel="测绘工程">测绘工程</li><li rel="地理信息科学">地理信息科学</li><li rel="遥感科学与技术">遥感科学与技术</li><li rel="生物医学工程">生物医学工程</li></ul></dd><dt>建筑与艺术学院</dt><dd style="display: none;"><ul><li rel="建筑学">建筑学</li><li rel="城市规划">城市规划</li><li rel="艺术设计">艺术设计</li><li rel="工业设计">工业设计</li><li rel="音乐表演">音乐表演</li></ul></dd><dt>航空航天学院</dt><dd style="display: none;"><ul><li rel="探测制导与控制技术">探测制导与控制技术</li><li rel="材料科学与工程">材料科学与工程</li></ul></dd><dt>物理与电子学院</dt><dd style="display: none;"><ul><li rel="应用物理学">应用物理学</li><li rel="电子信息科学与技术">电子信息科学与技术</li><li rel="光电信息科学与工程">光电信息科学与工程</li></ul></dd><dt>材料科学与工程学院</dt><dd style="display: none;"><ul><li rel="材料科学与工程">材料科学与工程</li></ul></dd><dt>湘雅医学院</dt><dd style="display: none;"><ul><li rel="药学">药学</li><li rel="预防医学">预防医学</li><li rel="护理学">护理学</li><li rel="精神医学">精神医学</li><li rel="麻醉学">麻醉学</li><li rel="临床医学">临床医学</li><li rel="生物科学">生物科学</li><li rel="医学信息学">医学信息学</li><li rel="医学检验">医学检验</li><li rel="口腔医学（五年制）">口腔医学（五年制）</li></ul></dd><dt>化学化工学院</dt><dd style="display: none;"><ul><li rel="化学工程与工艺">化学工程与工艺</li><li rel="制药工程">制药工程</li><li rel="应用化学">应用化学</li></ul></dd><dt>交通运输工程学院</dt><dd style="display: none;"><ul><li rel="交通运输">交通运输</li><li rel="交通设备信息工程">交通设备信息工程</li><li rel="物流工程">物流工程</li></ul></dd><dt>马克思主义学院</dt><dd style="display: none;"><ul><li rel="思想政治教育">思想政治教育</li></ul></dd><dt>资源加工与生物工程学院</dt><dd style="display: none;"><ul><li rel="矿物加工工程">矿物加工工程</li><li rel="无机非金属材料工程">无机非金属材料工程</li><li rel="生物工程">生物工程</li><li rel="生物技术">生物技术</li></ul></dd><dt>软件学院</dt><dd style="display: none;"><ul><li rel="软件工程">软件工程</li></ul></dd><dt>土木工程学院</dt><dd style="display: none;"><ul><li rel="土木工程">土木工程</li><li rel="工程力学">工程力学</li><li rel="工程管理">工程管理</li><li rel="消防工程">消防工程</li></ul></dd><dt>能源科学与工程学院</dt><dd style="display: none;"><ul><li rel="能源与动力工程">能源与动力工程</li><li rel="建筑环境与能源应用工程">建筑环境与能源应用工程</li><li rel="新能源科学与工程">新能源科学与工程</li></ul></dd><dt>商学院</dt><dd style="display: none;"><ul><li rel="国际经济与贸易">国际经济与贸易</li><li rel="金融学">金融学</li><li rel="信息管理与信息系统">信息管理与信息系统</li><li rel="工商管理">工商管理</li><li rel="会计学">会计学</li><li rel="财务管理">财务管理</li></ul></dd></dl></div> '];
        this.insertAdjacentHTML("beforeBegin", arraydemandhtml); //调用了js的方法插入html模板，并非jquery的方法，IE6好像有问题 
        document.getElementById("form1").insertAdjacentHTML("afterEnd", arrayselectdemandhtml);
        $("#selectmajor" + demandNumber).xMenu({
            width: 800,
            eventType: "click", //事件类型 支持focus click hover
            dropmenu: "#m" + demandNumber, //弹出层
            hiddenID: "Major" + demandNumber, //("#Major"),//隐藏域ID
            emptytext: "请选择"
        });

//        $(this).prev("demand").find("input[name='PositionName" + demandNumber + "']").rules("add", { required: true, messages: { required: "必须填写职位名称"} });
//        $(this).prev().find("textarea[name='Major" + demandNumber + "']").rules("add", { required: true, messages: { required: "必须选择需求专业"} });
//        $(this).prev().find("input[name='DemandNum" + demandNumber + "']").rules("add", { required: true, digits: true, messages: { required: "必须填写需求数量", digits: "请填写正整数"} });
        demandNumber++;
    });
});

$("#btnDel").live("click", function () {     //使用live方法可以为动态添加的空间绑定事件，而bind方法则不行
    $(this).parent().remove(); //移除第一个父元素，即demand元素
});

//专业不限制选择
function LimitMajorCheck(obj) {
    if ($(obj).attr("checked") == "checked") {
        $(obj).attr("checked", "true");
        $(obj).parent().parent().find("textarea[name = 'Major']").val($(obj).val()); //填写“不限制”
        //$(obj).prev("a").hide();
        $(obj).prev("a").css("visibility", "hidden");//隐藏仍然占据位置
    }
    else {
        $(obj).removeAttr("checked");
        $(obj).parent().parent().find("textarea[name = 'Major']").val(""); //空
        $(obj).prev("a").css("visibility", "visible");
    }
}

//删除附件逻辑
function deletefile(obj, filename) {
    //console.log(filename);
    obj.innerHTML = (obj.innerHTML == '删除' ? addfileshow() : addfilehide(filename));
}
function addfileshow() {//删除
    $("#addfile").attr("style", "display:block");
    $("input[name='IsDeleteFile']").val("yes");
    return '取消';
}
function addfilehide(filename) {//取消删除
    $("#addfile").attr("style", "display:none");
    $("input[name='IsDeleteFile']").val(filename);
    return '删除';
}



$(function () {
    $("textarea[name='Introduction']").wangEditor(
    {
        'menuConfig': [
            ['viewSourceCode'],
            ['fontFamily', 'fontSize'],
            ['bold', 'underline', 'italic'],
            ['setHead', 'foreColor', 'backgroundColor', 'removeFormat'],
            ['indent', 'outdent'],
            ['unOrderedList', 'orderedList'],
            ['justifyLeft', 'justifyCenter', 'justifyRight'],
            ['createLink', 'unLink'],
            ['insertHr', 'insertTable'],
            ['undo', 'redo'],
            ['fullScreen']
        ]
    });

    $("textarea[name='ContactInfo']").wangEditor(
    {
        'menuConfig': [
            ['viewSourceCode'],
            ['fontFamily', 'fontSize'],
            ['bold', 'underline', 'italic'],
            ['setHead', 'foreColor', 'backgroundColor', 'removeFormat'],
            ['indent', 'outdent'],
            ['unOrderedList', 'orderedList'],
            ['justifyLeft', 'justifyCenter', 'justifyRight'],
            ['createLink', 'unLink'],
            ['insertHr', 'insertTable'],
            ['undo', 'redo'],
            ['fullScreen']
        ]
    });
});

//用于调节显示
$(function () {
//    $("label").each(function () {
//        $(this).html($(this).html() + "&nbsp;&nbsp;&nbsp;&nbsp;");
    //    });
    $("textarea[name='Introduction']").css("width", "0px").css("height", "0px").css("display", "block").css("visibility", "hidden");
});



jQuery.extend(jQuery.validator.messages, {
    required: "",
    remote: "请修正该字段",
    email: "请输入正确格式的电子邮件",
    url: "请输入合法的网址",
    date: "请输入合法的日期",
    dateISO: "请输入合法的日期 (ISO).",
    number: "",
    digits: "",
    creditcard: "请输入合法的信用卡号",
    equalTo: "请再次输入相同的值",
    accept: "请输入拥有合法后缀名的字符串",
    maxlength: jQuery.validator.format("请输入一个 长度最多是 {0} 的字符串"),
    minlength: jQuery.validator.format("请输入一个 长度最少是 {0} 的字符串"),
    rangelength: jQuery.validator.format("请输入 一个长度介于 {0} 和 {1} 之间的字符串"),
    range: jQuery.validator.format("请输入一个介于 {0} 和 {1} 之间的值"),
    max: jQuery.validator.format("请输入一个最大为{0} 的值"),
    min: jQuery.validator.format("请输入一个最小为{0} 的值")
});

//表单验证
var validator;
$(document).ready(function () {
    $.validator.setDefaults({
        //debug: true
    });

    validator = $("#form1").validate({
        rules: {
//            Title: {
//                required: true
//            },
//            Introduction: {
//                required: true
//            }
//            ,
//            PositionName: {
//                required: true
//            },
//            Major: {
//                required: true
//            },
//            DemandNum: {
//                required: true,
//                digits: true
//            }
        },
        messages: {
//            Title: {
//                required: "必须填写文章标题"
//            },
//            Introduction: {
//                required: "必须填写公司简介"
//            }
//            ,
//            PositionName: {
//                required: "必须填写职位名称"
//            },
//            Major: {
//                required: "必须选择需求专业"
//            },
//            DemandNum: {
//                required: "必须填写需求数量",
//                digits: "请填写正整数"
//            }
        },
        submitHandler: function () {
            //另外写的一个函数提交  因为有这个问题出现 http://www.cnblogs.com/Fred_Xu/archive/2012/03/21/have-to-click-twice-to-get-submitted-by-using-submitHandler.html
        },
        invalidHandler: function (event, validator) {
            // '验证不通过的调用
            var errors = validator.numberOfInvalids();
            console.log(errors);
        }
    });
});

//实时检测输入框状态
//$("input[name='Title']").bind('input propertychange', function () {
//    var Title = $("input[name='Title']");
//    //var nextEle = $(this).nextAll(".demand-error");
//    if (Title != null || Title.val().trim() == "") {
//        var nextEle = Title.nextAll(".demand-error");
//        if (nextEle.length > 0) {
//            nextEle.css("display", "inline");
//        }
//        else {
//            $(this).after('<label class="demand-error">必须填写文章标题</label>');
//        }
//        flag = false;
//    }
//    else {
//        var nextEle = Title.nextAll(".demand-error");
//        if (nextEle.length > 0) {
//            nextEle.css("display", "none");
//        }
//    }
//});
$("input[name='Title']").bind('input propertychange', function () {
    var nextEle = $(this).nextAll(".demand-error");
    if ($(this).val().trim() == "") {
        if (nextEle.length > 0) {
            nextEle.css("display", "inline");
        }
        else {
            $(this).after('<label class="demand-error">必须填写文章标题</label>');
        }
        flag = false;
    }
    else {
        if (nextEle.length > 0) {
            nextEle.css("display", "none");
        }
    }
})
$("textarea[name='Introduction']").live('input propertychange', function () {
    var Introduction = $("textarea[name='Introduction']");
    if (Introduction != null || Introduction.val().trim() == "") {
        var nextEle = Introduction.nextAll(".demand-error");
        if (nextEle.length > 0) {
            nextEle.css("display", "inline");
        }
        else {
            $(this).after('<label class="demand-error">必须填写公司简介</label>');
        }
        flag = false;
    }
    else {
        var nextEle = Introduction.nextAll(".demand-error");
        if (nextEle.length > 0) {
            nextEle.css("display", "none");
        }
    }
});

$("input[name='PositionName']").live('input propertychange', function () {
    var nextEle = $(this).nextAll(".demand-error");
    if ($(this).val().trim() == "") {
        if (nextEle.length > 0) {
            nextEle.css("display", "inline");
        }
        else {
            $(this).after('<label class="demand-error">必须填写职位名称</label>');
        }
        flag = false;
    }
    else {
        if (nextEle.length > 0) {
            nextEle.css("display", "none");
        }
    }
});

$("textarea[name='Major']").live('input propertychange', function () {
    var nextEle = $(this).nextAll(".demand-error");
    if ($(this).val().trim() == "") {
        if (nextEle.length > 0) {
            nextEle.css("display", "inline");
        }
        else {
            $(this).after('<label class="demand-error">必须选择需求专业</label>');
        }
        flag = false;
    }
    else {
        if (nextEle.length > 0) {
            nextEle.css("display", "none");
        }
    }
});

$("input[name='DemandNum']").live('input propertychange', function () {
    var nextEle = $(this).nextAll(".demand-error");
    if ($(this).val().trim() == "" || isNaN($(this).val()) || $(this).val().trim() < 0) {  //isNaN()判断是不是数字，是返回false，不是返回true
        if (nextEle.length > 0) {
            nextEle.css("display", "inline");
        }
        else {
            $(this).after('<label class="demand-error">必须填写需求数量</label>');
        }
        flag = false;
    }
    else {
        if (nextEle.length > 0) {
            nextEle.css("display", "none");
        }
    }
});

function check() {
    var flag = true;
    //文章部分的判断
    var Title = $("input[name='Title']");
    if (!Title || Title.val().trim() == "") {
        var nextEle = Title.nextAll(".demand-error");
        if (nextEle.length > 0) {
            nextEle.css("display", "inline");
        }
        else {
            Title.after('<label class="demand-error">必须填写文章标题</label>');
        }
        flag = false;
    }
    else {
        var nextEle = Title.nextAll(".demand-error");
        if (nextEle.length > 0) {
            nextEle.css("display", "none");
        }
    }

    var Introduction = $("textarea[name='Introduction']");
    if (!Introduction || Introduction.val().trim() == "") {
        var nextEle = Introduction.nextAll(".demand-error");
        if (nextEle.length > 0) {
            nextEle.css("display", "inline");
        }
        else {
            Introduction.after('<label class="demand-error">必须填写公司简介</label>');
        }
        flag = false;
    }
    else {
        var nextEle = Introduction.nextAll(".demand-error");
        if (nextEle.length > 0) {
            nextEle.css("display", "none");
        }
    }

    //需求部分的判断
    $("input[name='PositionName']").each(function () {
        var nextEle = $(this).nextAll(".demand-error");
        if ($(this).val().trim() == "") {
            if (nextEle.length > 0) {
                nextEle.css("display", "inline");
            }
            else {
                $(this).after('<label class="demand-error">必须填写职位名称</label>');
            }
            flag = false;
        }
        else {
            if (nextEle.length > 0) {
                nextEle.css("display", "none");
            }
        }
    });

    $("textarea[name='Major']").each(function () {
        var nextEle = $(this).nextAll(".demand-error");
        if ($(this).val().trim() == "") {
            if (nextEle.length > 0) {
                nextEle.css("display", "inline");
            }
            else {
                $(this).after('<label class="demand-error">必须选择需求专业</label>');
            }
            flag = false;
        }
        else {
            if (nextEle.length > 0) {
                nextEle.css("display", "none");
            }
        }
    });

    $("input[name='DemandNum']").each(function () {
        var nextEle = $(this).nextAll(".demand-error");
        if ($(this).val().trim() == "" || isNaN($(this).val()) || $(this).val().trim() < 0) {  //isNaN()判断是不是数字，是返回false，不是返回true
            if (nextEle.length > 0) {
                nextEle.css("display", "inline");
            }
            else {
                $(this).after('<label class="demand-error">必须填写需求数量</label>');
            }
            flag = false;
        }
        else {
            if (nextEle.length > 0) {
                nextEle.css("display", "none");
            }
        }
    });

    return flag;
}
