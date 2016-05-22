
//判断大型招聘会
//TypeID逻辑
$(function () {
    $("#bigarticle2").hide();
    $("#bigarticle3").hide();
    $('#TypeID').change(function () {
        var typeid_selected = $(this).children('option:selected').val();
        if (typeid_selected == "1") {
            $("#TypeID_1").show();
            $("#bigarticle2").hide();
            $("#bigarticle3").hide();
        }
        else if (typeid_selected == "2") {
            $("#bigarticle2").show();
            $("#bigarticle3").hide();
            $("#TypeID_1").hide();
        }
        else if (typeid_selected == "3") {
            $("#bigarticle3").show();
            $("#bigarticle2").hide();
            $("#TypeID_1").hide();
        }        
        else {
            $("#bigarticle2").hide();
            $("#bigarticle3").hide();
            $("#TypeID_1").hide();
        }
    });
});


$(function () {
    $('#myModal').modal({
        backdrop: 'static',
        keyboard: false,
        show: false
    });
});
//提交表单
var lock = false; //防止重复提交
$(function () {
    var form = $("#form1");
    //提交表单 
    form.submit(function () {
        if (lock) { return false; }

        var options = {
            beforeSubmit: function () {//各种判断字段是否符合规格
                if (check()) {//form.valid() &&                    
                    $('#myModal').modal('show');
                    return true; //符合
                }
                return false; //不符合
            },
            url: "Create",
            type: "POST",
            data: $(this).formSerialize(), //$("#form1").fieldSerialize()
            dataType: "json", //这里是指控制器处理后返回的类型，这里返回json格式。  

            success: function (result) {
                if (result.message == "success") {
                    //alert("提交成功");
                    $('#myModal').find('.modal-body').html('提交成功');
                    window.location.replace("AddSuccess/1"); //跳转到提示页面
                }
                else {
                    //alert("ajax请求失败");
                    //$('#myModal').find('.modal-body').html('提交失败：' + result.message);
                    $('#myModal').modal('hide');
                    alert("提交失败：" + result.message);
                }
                //alert(result.message);
            },

            error: function (XMLResponse) {
                alert(XMLResponse.responseText);
                //这里是错误处理，通过这个alert可以看到错误的信息。对于调试来说是比较重要的哦。  
            }
        };
        lock = true;

        form.ajaxSubmit(options);
        lock = false;

        return false; //防止html自己提交表单
    })
});

$(function () {
    $("#selectmajor1").xMenu({
        width: 800,
        eventType: "click", //事件类型 支持focus click hover
        dropmenu: "#m1", //弹出层
        hiddenID: "Major1"//$(this).parent().next("#Major1")//("#Major"),//隐藏域ID		
    });
});