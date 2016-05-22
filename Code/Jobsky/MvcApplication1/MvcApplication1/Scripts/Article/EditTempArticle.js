
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
    //提交表单 
    var form = $("#form1");
    form.submit(function () {
        if (lock) { return; }

        var options = {
            beforeSubmit: function () {//各种判断字段是否符合规格
                if (check()) {
                    $('#myModal').modal('show');
                    return true; //符合
                }
                return false; //不符合
            },
            url: url, //editarticleid在view页面顶部定义了http://localhost:1719/Article/

            type: "POST",
            data: $(this).formSerialize(), //$("#form1").fieldSerialize()
            //target: "",
            dataType: "json", //这里是指控制器处理后返回的类型，这里返回json格式。  

            success: function (result) {
                if (result.message == "success") {
                    //alert("提交成功");
                    $('#myModal').find('.modal-body').html('提交成功');
                    window.location.replace(urlsuccess); //跳转到提示页面http://localhost:1719/Article/"AddSuccess/2"
                }
                else {
                    //alert("ajax请求失败");
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
        //$("#form1").ajaxForm(options);
        lock = true;

        form.ajaxSubmit(options);
        lock = false;

        return false; //防止html自己提交表单
    })
});
//初次加载的专业选择
$(function () {
    $('.topnav').each(function () {
        var arrayselectdemandhtml = ['<div id="m' + demandNumber + '" class="xmenu" style="display: none;">  <div class="select-info">  <label class="top-label">已选职位：</label>  <ul></ul>  <a name="menu-confirm" href="javascript:void(0);" class="a-btn"><span class="a-btn-text">确定</span>  </a>  </div>  <dl>  <dt>信息科学与工程学院</dt>  <dd style="display: none;">  <ul>  <li rel="自动化">自动化</li>  <li rel="电气工程及其自动化">电气工程及其自动化</li>  <li rel="测控技术与仪器">测控技术与仪器</li>  <li rel="电子信息工程">电子信息工程</li>  <li rel="通信工程">通信工程</li>  <li rel="计算机科学与技术">计算机科学与技术</li>  <li rel="信息安全">信息安全</li>  <li rel="物联网工程">物联网工程</li>  <li rel="智能科学与技术">智能科学与技术</li>  </ul>  </dd>  <dt>数学与统计学院</dt>  <dd style="display: none;">  <ul>  <li rel="数学与应用数学">数学与应用数学</li>  <li rel="信息与计算科学">信息与计算科学</li>  <li rel="统计学">统计学</li>  </ul>  </dd>  <dt>文学院</dt>  <dd style="display: none;">  <ul>  <li rel="汉语言文学">汉语言文学</li>  <li rel="广播电视学">广播电视学</li>  </ul>  </dd>  <dt>法学院</dt>  <dd style="display: none;">  <ul>  <li rel="法学">法学</li>  </ul>  </dd>  <dt>粉末冶金研究院</dt>  <dd style="display: none;">  <ul>  <li rel="材料化学">材料化学</li>  <li rel="粉体材料科学与工程">粉体材料科学与工程</li>  </ul>  </dd>  <dt>冶金与环境学院</dt>  <dd style="display: none;">  <ul>  <li rel="环境工程">环境工程</li>  <li rel="新能源材料与器件">新能源材料与器件</li>  </ul>  </dd>  <dt>机电工程学院</dt>  <dd style="display: none;">  <ul>    </ul>  </dd>  <dt>外国语学院</dt>  <dd style="display: none;">  <ul>  <li rel="英语">英语</li>  <li rel="法语">法语</li>  <li rel="日语">日语</li>  </ul>  </dd>  <dt>公共管理学院</dt>  <dd style="display: none;">  <ul>  <li rel="行政管理">行政管理 </li>  <li rel="社会学">社会学</li>  <li rel="劳动与社会保障">劳动与社会保障</li>  </ul>  </dd>  <dt>资源与安全工程学院</dt>  <dd style="display: none;">  <ul>  <li rel="采矿工程">采矿工程</li>  <li rel="安全工程">安全工程</li>  <li rel="城市地下空间工程">城市地下空间工程</li>  </ul>  </dd>  <dt>地球科学与信息物理学院</dt>  <dd style="display: none;">  <ul>  <li rel="地质工程">地质工程</li>  <li rel="地球信息科学与技术">地球信息科学与技术</li>  <li rel="测绘工程">测绘工程</li>  <li rel="地理信息科学">地理信息科学</li>  <li rel="遥感科学与技术">遥感科学与技术</li>  <li rel="生物医学工程">生物医学工程</li>  </ul>  </dd>  <dt>建筑与艺术学院</dt>  <dd style="display: none;">  <ul>  <li rel="建筑学">建筑学</li>  <li rel="城市规划">城市规划</li>  <li rel="艺术设计">艺术设计</li>  <li rel="工业设计">工业设计</li>  <li rel="音乐表演">音乐表演</li>  </ul>  </dd>  <dt>航空航天学院</dt>  <dd style="display: none;">  <ul>  <li rel="航空航天工程">航空航天工程</li>  <li rel="探测制导与控制技术">探测制导与控制技术</li>  <li rel="材料科学与工程">材料科学与工程</li>  </ul>  </dd>  <dt>物理与电子学院</dt>  <dd style="display: none;">  <ul>  <li rel="应用物理学">应用物理学</li>  <li rel="电子信息科学与技术">电子信息科学与技术</li>  </ul>  </dd>  <dt>材料科学与工程学院</dt>  <dd style="display: none;">  <ul>  <li rel="材料科学与工程">材料科学与工程</li>  </ul>  </dd>  <dt>湘雅医学院</dt>  <dd style="display: none;">  <ul>  <li rel="药学">药学</li>  <li rel="预防医学">预防医学</li>  <li rel="护理学">护理学</li>  <li rel="精神医学">精神医学</li>  <li rel="麻醉学">麻醉学</li>  <li rel="临床医学">临床医学</li>  <li rel="生物科学">生物科学</li>  <li rel="医学信息类">医学信息类</li>  <li rel="医学检验">医学检验</li>  <li rel="口腔医学">口腔医学</li>  </ul>  </dd>  <dt>化学化工学院</dt>  <dd style="display: none;">  <ul>  <li rel="化学工程与工艺">化学工程与工艺</li>  <li rel="制药工程">制药工程</li>  <li rel="应用化学">应用化学</li>  </ul>  </dd>  <dt>交通运输工程学院</dt>  <dd style="display: none;">  <ul>  <li rel="交通运输">交通运输</li>  <li rel="交通设备信息工程">交通设备信息工程</li>  <li rel="物流工程">物流工程</li>  </ul>  </dd>  <dt>马克思主义学院</dt>  <dd style="display: none;">  <ul>  <li rel="思想政治教育">思想政治教育</li>  </ul>  </dd>  <dt>资源加工与生物工程学院</dt>  <dd style="display: none;">  <ul>  <li rel="矿物加工工程">矿物加工工程</li>  <li rel="无机非金属材料工程">无机非金属材料工程</li>  <li rel="生物工程">生物工程</li>  <li rel="生物技术">生物技术</li>  </ul>  </dd>  <dt>软件学院</dt>  <dd style="display: none;">  <ul>  <li rel="软件工程">软件工程</li>  </ul>  </dd>  <dt>土木工程学院</dt>  <dd style="display: none;">  <ul>  <li rel="土木工程">土木工程</li>  <li rel="工程力学">工程力学</li>  <li rel="工程管理">工程管理</li>  <li rel="消防工程">消防工程</li>  </ul>  </dd>  <dt>能源科学与工程学院</dt>  <dd style="display: none;">  <ul>  <li rel="能源与动力工程">能源与动力工程</li>  <li rel="建筑环境与能源应用工程">建筑环境与能源应用工程</li>  <li rel="新能源科学与工程">新能源科学与工程</li>  </ul>  </dd>  <dt>商学院</dt>  <dd style="display: none;">  <ul>  <li rel="国际经济与贸易">国际经济与贸易</li>  <li rel="金融学">金融学</li>  <li rel="信息管理与信息系统">信息管理与信息系统</li>  <li rel="工商管理">工商管理</li>  <li rel="会计学">会计学</li>  <li rel="财务管理">财务管理</li>  <li rel="电子商务">电子商务</li>  </ul>  </dd>  </dl> </div> '];
        document.getElementById("form1").insertAdjacentHTML("afterEnd", arrayselectdemandhtml);

        $(this).children('#selectmajor1').xMenu({
            width: 800,
            eventType: "click", //事件类型 支持focus click hover
            dropmenu: "#m" + demandNumber, //弹出层
            hiddenID: "Major" + demandNumber   //$(this).parent().next("#Major1")//("#Major"),//隐藏域ID		
        });
        demandNumber++;
    });
});