#ifndef HEADER_PRIME
#define HEADER_PRIME

#include<iostream>
using namespace std;

int num;      //n由用户输入，为素数环的个数
int a[20];      //数组存储20个数
bool isUsed[20];//标记是否已放入素数环中

class Prime
{
public:
	
	void init(int a[],bool isUsed[]){
		for (int i = 0; i < 20; i++){       //初始化数组a[],isused[]
			a[i] = i + 1;
			isUsed[i] = false;
		}
	}

	bool isPrime(int m){                     //判断是否为素数
		if (m<3)
			return false;
		int len = (int)sqrt(m + 0.0);
		for (int i = 2; i <= len; i++){
			if (m%i == 0)
				return false;
		}
		return true;
	}

	
	void Ring(int cur){                                   //递归输出所有素数环
		
		if (cur == num&&isPrime(a[0] + a[num - 1])){      //在最后一层执行,输出当前求得解答串  
			for (int i = 0; i<num; i++)
				cout << a[i] << ' ';
			cout << endl;
			return;
		}
		
		else for (int i = 2; i <= num; i++){              //前n-1层执行,递归选定每一层的整数,使其与前一层的整数之和为素数  
			if (!isUsed[i] && isPrime(i + a[cur - 1])){   //当前值i没被使用,且与前一个选定值之和为素数  
				a[cur] = i;                               //选i为当前项值  
				isUsed[i] = true;                         //状态从没被使用改为被使用  
				Ring(cur + 1);                            //进入下一层
				isUsed[i] = false;                        //递归后面的语句在从n-1层到第1层回调时执行  
				                                          //状态还原,使重新求下一个有效串时不被干扰  
			}
		}
	}
	void PrintRing(){
		init(a, isUsed);
		cout << "请输入素数环的数字个数(可测试多组数据，0为退出，建议数小点，素数环个数较少3，4，8)：";
		cin >> num;
		while (cin >> num, num){
			Ring(1);
		}
		
	}
};

#endif