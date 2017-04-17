var WIDTH=document.body.clientWidth*0.8;  //根据屏幕大小计算锁屏图案宽度
var HEIGHT=WIDTH;
var RADIUS=WIDTH/12;   //密码点圆圈半径
var r=RADIUS/3;
var pointArr=[];       //存储所有密码点位置
var pointWait=[];      //用来记录未被绘制的密码点
var pointTouched=[];   //用来记录未被选中的点，同一个点不能被重复绘制
var pwdStorage=new Object();  //存储密码
pwdStorage.statusFlag=2;

var activeFlag=false; //用来判断当前的行为，当用户点击之后，其值为true，代表当前正在绘制图案，其值为false时，不进行任何绘制图形动作


var R=15;
var flag1=false;     //开关标识
var flag2=false;

window.onload=function(){
	//时间设置
	var time=new Date();
	var T=time.getHours()+":"+time.getMinutes();
	document.getElementById('time').innerHTML=T;

	//初始化开关样式
	setSwtCSS();
	drawBattery();

	//判断屏幕大小，若为PC端则重新设置宽度
	if(document.body.clientWidth>800){
		var phone=document.getElementById('phone');
		phone.style.width=320+"px";
		phone.style.height=600+"px";
		// phone.style.marginLeft="30%";
		// phone.style.marginTop="5%";
		phone.style.marginLeft="10px";
		phone.style.border="solid 1px black";
		phone.style.backgroundColor="LightCyan";

		var phoneBody=document.getElementById('phoneBody');
		phoneBody.style.width=340+"px";
		phoneBody.style.height=640+"px";
		phoneBody.style.width=340+"px";
		phoneBody.style.marginTop="2%";
		phoneBody.style.paddingTop="10px"
		phoneBody.style.marginLeft="40%";
		phoneBody.style.backgroundColor="white";
		phoneBody.style.borderRadius="15px";
		
		
		WIDTH=320;
		HEIGHT=WIDTH;
		RADIUS=WIDTH/12;
		r=RADIUS/3;
	}

	//判断localStorage中是否已存储密码
	if(window.localStorage.getItem('password')){
			pwdStorage.statusFlag=2;
			pwdStorage.pwdFirst=JSON.parse(window.localStorage.getItem('password'));
		}

	//获取canvas元素
	var convas=document.getElementById('canvas');
	var context=convas.getContext("2d");
	convas.width=WIDTH;
	convas.height=HEIGHT;
	

	//初始化锁屏图案、开关状态
	circlesInit(context,convas);
	eventListener(convas,context);
	openOrClose_2();
	drawSignal();
	setInterval("drawSignal()",1000);
}

/*电池绘制*/
function drawBattery(){
	var btyCvs=document.getElementById('battery');
	var btyCxt=btyCvs.getContext("2d");

	btyCvs.width=30;
	btyCvs.height=14;
	btyCxt.beginPath()
	btyCxt.lineWidth=2
	btyCxt.strokeStyle="white"
	btyCxt.moveTo(1,1)
	btyCxt.lineTo(27,1)
	btyCxt.lineTo(27,13)
	btyCxt.lineTo(1,13)
	btyCxt.closePath()
	btyCxt.stroke()

	btyCxt.beginPath()
	btyCxt.fillStyle="white"
	btyCxt.moveTo(4,4)
	btyCxt.lineTo(24,4)
	btyCxt.lineTo(24,10)
	btyCxt.lineTo(4,10)
	btyCxt.closePath()
	btyCxt.fill()

	btyCxt.beginPath()
	btyCxt.fillStyle="white"
	btyCxt.arc(29,7,3,0,2*Math.PI)
	btyCxt.closePath()
	btyCxt.fill()


}

/*手机信号显示函数*/
function drawSignal(){
	var rad=15;
	var sgnNum=Math.floor(Math.random()*10+1)/2;
	var sgnCvs=document.getElementById('signal');
	var sgnCxt=sgnCvs.getContext("2d");
	sgnCvs.width=rad*15;
	// sgnCvs.height="100%";
	// sgnCvs.padding="5px auto";
	sgnCxt.clearRect(0,0,sgnCxt.width,sgnCxt.height)
	sgnCxt.beginPath()
	sgnCxt.fillStyle="white"
	for(var i=0;i<sgnNum;i++){
		sgnCxt.arc(rad+rad+3*rad*i,rad+rad,rad,0,2*Math.PI)
	}
	sgnCxt.fill()
	sgnCxt.closePath()

	var time=new Date();
	var T=time.getHours()+":"+time.getMinutes();
	document.getElementById('time').innerHTML=T;
}

/*开关初始化函数*/
function setSwtCSS(){
	var swt1=document.getElementById('switch1');
	swt1.style.width=3*R+R/2+2+"px";
	swt1.style.height=2*R+"px";	
	swt1.style.textAlign="center";
	swt1.style.marginLeft="20%";
	swt1.style.backgroundColor="gainsboro";
	swt1.style.borderRadius=R+"px";
	var crc1=swt1.children[0];
	crc1.style.width=2*R+"px";
	crc1.style.height=2*R+"px";
	crc1.style.backgroundColor="white";
	crc1.style.marginLeft="1px";
	crc1.style.borderRadius=R+"px";

	var swt2=document.getElementById('switch2');
	swt2.style.width=3*R+R/2+2+"px";
	swt2.style.height=2*R+"px";	
	swt2.style.textAlign="center";
	swt2.style.marginLeft="20%";
	swt2.style.backgroundColor="gainsboro";
	swt2.style.borderRadius=R+"px";
	var crc=swt2.children[0];
	crc.style.width=2*R+"px";
	crc.style.height=2*R+"px";
	crc.style.backgroundColor="white";
	crc.style.marginLeft="1px";
	crc.style.borderRadius=R+"px";


}

/*关闭或打开解锁开关*/
function openOrClose_2(){
	if(flag2==false){
		flag2=true;
		var swt2=document.getElementById('switch2');
		var crc=swt2.children[0];
		swt2.style.backgroundColor="SpringGreen";
		crc.style.marginLeft=R+R/2+"px";
		// for(var i=1;i<R+R/2;i++){
		// 	setTimeout("");
		// 	crc.style.marginLeft=i+"px";
		// }
		var msg=document.getElementById('message');
		msg.innerHTML="小主，请解锁~";
		pwdStorage.statusFlag=2;
	}else{
		flag2=false;
		var swt2=document.getElementById('switch2');
		var crc=swt2.children[0];
		swt2.style.backgroundColor="gainsboro";
		crc.style.marginLeft="1px";
	}

	if(flag1==true){
		flag1=false;
		var swt1=document.getElementById('switch1');
		var crc=swt1.children[0];
		swt1.style.backgroundColor="gainsboro";
		crc.style.marginLeft="1px";
	}
}

/*关闭或打开设置密码开关*/
function openOrClose_1(){
	if(flag1==false){
		flag1=true;
		var swt1=document.getElementById('switch1');
		var crc=swt1.children[0];
		swt1.style.backgroundColor="SpringGreen";
		crc.style.marginLeft=R+R/2+"px";
		// for(var i=1;i<R+R/2;i++){
		// 	setTimeout("");
		// 	crc.style.marginLeft=i+"px";
		// }
		var msg=document.getElementById('message');
		msg.innerHTML="请输入手势密码";
		pwdStorage.statusFlag=0;
	}else{
		flag1=false;
		var swt1=document.getElementById('switch1');
		var crc=swt1.children[0];
		swt1.style.backgroundColor="gainsboro";
		crc.style.marginLeft="1px";
	}
	if(flag2==true){
		flag2=false;
		var swt2=document.getElementById('switch2');
		var crc=swt2.children[0];
		swt2.style.backgroundColor="gainsboro";
		crc.style.marginLeft="1px";
	}
}

/*初始化屏幕图案*/
function circlesInit(cxt,cvs){

	cxt.clearRect(0,0,cvs.width,cvs.height)
	pointArr=[];
	pointTouched=[];
	pointWait=[];
	for(var i=0;i<3;i++){
		for(var j=0;j<3;j++){
			var objPos=new Object();
			objPos.x=j*3*RADIUS+3*RADIUS;
			objPos.y=i*3*RADIUS+3*RADIUS;
			
			pointArr.push(objPos);
			pointWait.push(objPos);
			cxt.beginPath()
			cxt.arc(j*3*RADIUS+3*RADIUS,i*3*RADIUS+3*RADIUS,RADIUS,0,2*Math.PI)
			cxt.closePath()
			cxt.lineWidth=1			//圆圈线的宽度
			cxt.strokeStyle="black"	//圆圈颜色
			cxt.stroke()
		}
	}
	pointWait=pointArr;
}

/*获取手指的位置---手机端*/
function getTouchtedPos(event){
	//得到当前所在元素的上下左右的数据
	var data=event.currentTarget.getBoundingClientRect();
	var dis=new Object();

	//获得鼠标相对于convas的坐标位置
	dis.x=event.touches[0].clientX-data.left;
	dis.y=event.touches[0].clientY-data.top;
	return dis;
}

/*获取鼠标位置---PC端*/
function getMousePos(event){
	var dis=new Object();
	dis.x=event.offsetX;
	dis.y=event.offsetY;
	console.log(dis);
	return dis;
}

/*监听函数，监听用户操作*/
function eventListener(cvs,cxt){
	//PC端鼠标点击
	cvs.addEventListener("mousedown",function(e){
		e.preventDefault();  //阻止个别系统屏幕滚动
		var pos=getMousePos(e);

		//判断当前选中的元素,绘制实心点
		for (var i = 0 ; i < pointArr.length ; i++) {
                    if (Math.abs(pos.x - pointArr[i].x) < RADIUS && Math.abs(pos.y - pointArr[i].y) < RADIUS) {
                        activeFlag = true;
                       	drawPoint(pointArr[i].x,pointArr[i].y,cxt);
                       	pointTouched.push(pointArr[i]);
                        pointWait.splice(i,1);
                        break;
                    }
        }
	},false);

	//手机端手指点击屏幕
	cvs.addEventListener("touchstart",function(e){
		e.preventDefault();  //阻止个别系统屏幕滚动
		var pos=getTouchtedPos(e);
		//判断当前选中的元素,绘制实心点
		for (var i = 0 ; i < pointArr.length ; i++) {
                    if (Math.abs(pos.x - pointArr[i].x) < RADIUS && Math.abs(pos.y - pointArr[i].y) < RADIUS) {
 
                        activeFlag = true;
                       	drawPoint(pointArr[i].x,pointArr[i].y,cxt);
                       	pointTouched.push(pointArr[i]);
                        pointWait.splice(i,1);
                        break;
                    }
        }
	},false);

	//PC端鼠标移动
	cvs.addEventListener("mousemove",function(event){
		event.preventDefault();
		//当前用户正在绘制图案
		if(activeFlag){
			var pos=getMousePos(event);
			drawAll(pos,cxt,cvs);
			
		}
	},false);
	//手机端手指移动
	cvs.addEventListener("touchmove",function(event){
		event.preventDefault();
		//当前用户正在绘制图案
		if(activeFlag){
			var pos=getTouchtedPos(event);
			drawAll(pos,cxt,cvs);
			
		}
	},false);

	//PC端鼠标离开屏幕
	document.body.addEventListener("mouseup",function(event){
		if(activeFlag){
			activeFlag=false;
			storePwdToLocal(pointTouched);
			//半秒后清空屏幕
			setTimeout(function(){

			circlesInit(cxt,cvs);
			},200);
		}
	},false);
	//手机端手指离开屏幕
	cvs.addEventListener("touchend",function(event){
		if(activeFlag){
			activeFlag=false;
			storePwdToLocal(pointTouched);
			//半秒后清空屏幕
			setTimeout(function(){

			circlesInit(cxt,cvs);
			},200);
		}
	},false);
	document.addEventListener('touchmove',function(event){
		event.preventDefault();
	},false);
}

//将用户绘制密码保存入localStorage
function storePwdToLocal(pwd){
	
	var msg=document.getElementById('message');
	//若用户正在绘制密码
	if(pwdStorage.statusFlag==0){
		if(pointTouched.length<5){
			msg.innerHTML="密码太短，至少需要5个点";
		}else{
			pwdStorage.statusFlag=1;
			pwdStorage.pwdFirst=pwd;
			//请确认密码
			msg.innerHTML="请确认密码";
			changeColor("SpringGreen");
		}
	}
	//若用户第二次输入密码，确认密码
	else if(pwdStorage.statusFlag==1){
		var msg=document.getElementById('message');
		if(JSON.stringify(pwdStorage.pwdFirst)==JSON.stringify(pwd)){

			pwdStorage.statusFlag=2;
			window.localStorage.setItem('password',JSON.stringify(pwd));
			//加提示，用户设置密码成功
			msg.innerHTML="密码设置成功";
			//切换解锁状态
			openOrClose_2();
			//将颜色显示为通过色
			changeColor("SpringGreen");
		}
		else{
			// console.log(pwdStorage.pwdFirst);
			// console.log(pwd);
			//加提示，输入不一致，请重新输入
			msg.innerHTML="两次输入不一致,请重新输入";
			pwdStorage.statusFlag=0;
			//将颜色显示为警告色
			changeColor("red");
		}
	}
	//若用户正在解锁
	else {
		if(!window.localStorage.getItem('password')){
			msg.innerHTML="请先设置密码";
		}else if(window.localStorage.getItem('password')==JSON.stringify(pwd)){
			//加提示用户解锁成功
			msg.innerHTML="密码正确";
			changeColor("SpringGreen");
		}
		else{
			//加提示，解锁失败
			msg.innerHTML="密码输入错误";
			//将颜色显示为警告色
			changeColor("red");
		}
	}
}

/*当用户密码输入正确货错误时，改变图案上密码点的颜色，作为提示*/
function changeColor(col){
	var convas=document.getElementById('canvas');
	var context=convas.getContext("2d");
	
	for(var i=0;i<pointTouched.length;i++){
		context.beginPath()
		context.lineWidth=2
		context.strokeStyle=col;
		context.arc(pointTouched[i].x,pointTouched[i].y,RADIUS,0,2*Math.PI)
		context.stroke()
		context.closePath()
	}
	
}

/*点绘制函数*/
function drawPoint(x,y,cxt){
	cxt.beginPath()
	cxt.arc(x,y,r,0,2*Math.PI)
	cxt.closePath()
	cxt.fillStyle="black"
	cxt.fill()

	cxt.beginPath()
	cxt.arc(x,y,RADIUS,0,2*Math.PI)
	cxt.closePath()
	cxt.strokeStyle="black"
	cxt.stroke()
	
}

/*圆圈绘制函数*/
function singleCircle(x,y,cxt){
	cxt.beginPath()
	cxt.arc(x,y,RADIUS,0,2*Math.PI)
	cxt.strokeStyle="black"
	cxt.closePath()
	cxt.stroke()
}

/*点与点之间连线绘制函数*/
function drawLine(x,y,cxt){
	cxt.beginPath()
	cxt.lineStyle="black"
	cxt.lineWidth=1
	cxt.moveTo(pointTouched[0].x,pointTouched[0].y)
	for (var i=1;i<pointTouched.length;i++){
		cxt.lineTo(pointTouched[i].x,pointTouched[i].y)

	}
	cxt.lineTo(x,y)
	cxt.stroke()
}

/*用户密码绘制或输入完成时，图案绘制结束*/
function drawAll(pos,cxt,cvs){
	cxt.clearRect(0,0,cvs.width,cvs.height)

	//绘制圈图案
	for(var i=0;i<pointArr.length;i++){
		singleCircle(pointArr[i].x,pointArr[i].y,cxt);
	}

	//绘制用户选择点
	for(var i=0;i<pointTouched.length;i++){
		 drawPoint(pointTouched[i].x,pointTouched[i].y,cxt);
		
	}
	drawLine(pos.x,pos.y,cxt);

	for(var i=0;i<pointWait.length;i++){
		if(Math.abs(pos.x-pointWait[i].x)<RADIUS&&Math.abs(pos.y-pointWait[i].y)<RADIUS){
			drawLine(pointWait[i].x,pointWait[i].y,cxt);
			pointTouched.push(pointWait[i]);
			pointWait.splice(i,1);
			break;
		}
	}
}

