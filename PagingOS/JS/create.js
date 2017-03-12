/*全局变量 函数执行时间，缺页率*/
var funTime = [0,0,0];   //算法运行时间

var stopRate = [0,0,0];  //缺页率

var memNum;  //内存大小

var pageNum;  //指令页数

var pageSeries = new Array();

var resultFIFO = new Array(); //初始化一个对象，记录内存中的状态
var resultLRU = new Array();
var resultOPT = new Array();
var memContain = new Array();  //模拟一块内存

/*指令生成控制函数*/
$("#createNum").click(function() {
	memNum=parseInt($("#selMemNum option:selected").val())
	pageNum=parseInt($("#selPageNum option:selected").val())
	/* Act on the event */
	/*调用随机数生成函数*/
	var series=rndNum(pageNum);
	pageSeries=series.split(",");
	// alert(pageSeries.toString());
	$("#getSeries")[0].value=series;

	//调用内存控制模块函数
	//Memory();
});



/*指令生成模块 生成指定个数的随机数数值在0~9之间*/
function rndNum(n){   
    
var rnd="";   
    
for(var i=0;i<n;i++)   {

	tmp=Math.floor(Math.random()*10);

	rnd+=tmp+" , ";   
	
}

return rnd;   
    
} 

/*FIFO算法*/

function FIFO(memContainCurrent,pageSeriesCurrent,j){
	var mC=memContainCurrent.split(",");
	var pS=pageSeriesCurrent.split(",");
	var i=j;
	var countF=0;     //缺页中断次数
	var start = new Date().getTime();  //获取起始时间
	
	while(i<pageNum){     //还有未被访问的页面
		var k=0;
		var flag=0;
		while(k<memNum){
			if(mC[k].toString().trim()!=pS[i].toString().trim()){
				k++;  //与内存中的下一个页面比较
			}	
			else{
				flag=1;
				i++;  //当内存中存在时，取序列的下一个页面
				k=0;
				break;
			}
		}
		if(flag==0){
			//产生缺页中断
			for(var m=0;m<memNum-1;m++){
				mC[m]=mC[m+1];
			}  //内存中后一个页面替换前一个页面

			mC[memNum-1]=pS[i]; //新页面装入内存
			var memCurrent=mC.toString();
			resultFIFO.push(memCurrent); //将每次状态录入数组
			countF++;
			i++;
			k=0;
		}
		
	}
	
	var end = new Date().getTime();    //算法执行结束时间
	
	var myTime = parseInt(end -start); //计算出函数执行时间并转换为整数 ms为单位
	stopRate[0]=countF;
	funTime[0]=myTime;
}

/*FIFO算法*/

function LRU(memContainCurrent,pageSeriesCurrent,j){
	var mC=memContainCurrent.split(",");
	var pS=pageSeriesCurrent.split(",");
	var i=j;
	var countL=0;     //缺页中断次数
	var start = new Date().getTime();  //获取起始时间
	
	mC=mC.reverse();   //将内存中页号进行倒序
	var memCurrent=mC.toString();
	resultLRU.push(memCurrent); //将每次状态录入数组
	while(i<pageNum){
		var flag=0;
		flag=compare(mC,pS,i);
		if(flag!=-1){  //当内存中存在当前页面时
			var tmp = flag-1;
			while(tmp<memNum-1){ //若不在队尾则移到队尾
				mC[tmp]=mC[tmp+1];
				tmp++;
			}
			if(tmp==memNum-1){
				mC[tmp]=pS[i];  
			}
			i++; //继续访问下一个页面
		}
		else{ //产生缺页中断
			for(var m=0;m<memNum-1;m++){
				mC[m]=mC[m+1];
			}  //内存中后一个页面替换前一个页面

			mC[memNum-1]=pS[i]; //新页面装入内存
			var memCurrent=mC.toString();
			resultLRU.push(memCurrent); //将每次状态录入数组
			countL++;
			i++;
		}
	}
	var end = new Date().getTime();    //算法执行结束时间
	var myTime = parseInt(end -start); //计算出函数执行时间并转换为整数 ms为单位
	stopRate[1]=countL;
	funTime[1]=myTime;
}

/*FIFO算法*/

function OPT(memContainCurrent,pageSeriesCurrent,j){
	var mC=memContainCurrent.split(",");
	var pS=pageSeriesCurrent.split(",");
	var i=j;
	var countO=0;     //缺页中断次数

	var d=0; //d做指令序列的下标索引
	var m=0; //m做内存中已有页号的下标索引
	var dmax=0; //最大距离页面索引
	var dis=new Array();  //距离数组
	//初始化为全0
	for(var p=0;p<memNum;p++){
		dis[p]=100;
	}
	var start = new Date().getTime();  //获取起始时间
	
	while(i<pageNum){
		var flag=0;
		flag=compare(mC,pS,i);
		if(flag!=-1){  //当内存中存在当前页面时
			i++; //继续访问下一个页面
			// alert("星1");
		}
		else{ //产生缺页中断
			
			if(i==pageNum-1){ //如果后面没有指令
				
				i++;
				countO++;
			}else{
				
				m=0;
				while(m<memNum){  //
					
					d=i+1;
					while(d<pageNum){

						if(mC[m].toString().trim()==pS[d].toString().trim()){
							
							dis[m]=d-i;  //得到后面页号的距离
							break;
						}else{
							
							d++;
						}
					}
					m++;
				}
				dmax=dis.indexOf(Math.max.apply(Math,dis));  //得到距离最大的页面的在内存中的位置
				mC[dmax]=pS[i];
				var memCurrent=mC.toString();
				resultOPT.push(memCurrent); //将每次状态录入数组
				countO++;
				i++;
			}
			
		}
	}
	var end = new Date().getTime();    //算法执行结束时间
	var myTime = parseInt(end -start); //计算出函数执行时间并转换为整数 ms为单位

	stopRate[2]=countO;
	funTime[2]=myTime;
}

//比较模块函数
function compare(memContainCurrent,pageSeriesCurrent,j){
	//判断当前执行指令是否已存在内存中
	var k=0;
	var flag=-1
	while(k<memNum){
		if(memContainCurrent[k].toString().trim()!=pageSeriesCurrent[j].toString().trim()){
			k++;  //与内存中的下一个页面比较
		}	
		else{
			flag=k;
			return k;
		}
	}
	return flag;
}

/*将算法执行时间返回*/
function returnTime()
{
	return funTime;
}

// function returnFIFORes(){
// 	return resultFIFO;
// }

/*将算法缺页率返回*/
function returnRate()
{
	return stopRate;
}

/*内存控制模块函数*/
function Memory(){

	resultFIFO=[];
	resultLRU=[];
	resultOPT=[];
	memContain=[];
	var k=0;    //遍历内存时使用
	var i=1;  //i为内存当前可以装入的位置
	var j=1;  //j为从指令序列中取出指令的位置
    
	memContain[0]=pageSeries[0];   //将第一个指令页号装入内存
	while(i<memNum&&memContain.length<memNum){  //内存未装满时
		k=0;
		while(k<memContain.length){
			if(memContain[k].toString().trim()==pageSeries[j].toString().trim()){  //是否已存在
				j++;   //内存中存在当前页，继续读取下一个指令
				k=0;
				break;
			}else{
				if(k==i-1){   //当前是否可装入
					memContain[i]=pageSeries[j];
					/*每装入一次记录一次内存中的页号信息，将其转换为字符串进行存储*/
					var memCurrent=memContain.toString();
					resultFIFO.push(memCurrent); //将每次状态录入数组
					resultLRU.push(memCurrent);
					resultOPT.push(memCurrent);
					i++;
					j++;
					break;
				}
				else{  //继续比较
					k++;
				}
			}
			

		}
	}
	memContain1=memContain.toString();
	memContain2=memContain.toString();
	memContain3=memContain.toString();
	//调用FIFO算法
	FIFO(memContain1,pageSeries.toString(),j);
	LRU(memContain2,pageSeries.toString(),j);
	OPT(memContain3,pageSeries.toString(),j);
    
}

function FIFOReading(){
	//resultFIFO
	//alert(resultFIFO[2][3]);
	getFIFOElm=document.getElementsByClassName('allItems')[0].children;
	for(var m=0;m<resultFIFO.length;m++){
		var item=resultFIFO[m].split(",");
		for(var n=0;n<item.length;n++){
			getFIFOElm[m].children[n].innerHTML=item[n];
		}
	}
	getLRUElm=document.getElementsByClassName('allItems')[1].children;
	for(var m=0;m<resultLRU.length;m++){
		var item=resultLRU[m].split(",");
		for(var n=0;n<item.length;n++){
			getLRUElm[m].children[n].innerHTML=item[n];
		}
	}
	getOPTElm=document.getElementsByClassName('allItems')[2].children;
	for(var m=0;m<resultOPT.length;m++){
		var item=resultOPT[m].split(",");
		for(var n=0;n<item.length;n++){
			getOPTElm[m].children[n].innerHTML=item[n];
		}
	}

}

