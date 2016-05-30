#ifndef HEADER_PRIME
#define HEADER_PRIME

#include<iostream>
using namespace std;

int num;      //n���û����룬Ϊ�������ĸ���
int a[20];      //����洢20����
bool isUsed[20];//����Ƿ��ѷ�����������

class Prime
{
public:
	
	void init(int a[],bool isUsed[]){
		for (int i = 0; i < 20; i++){       //��ʼ������a[],isused[]
			a[i] = i + 1;
			isUsed[i] = false;
		}
	}

	bool isPrime(int m){                     //�ж��Ƿ�Ϊ����
		if (m<3)
			return false;
		int len = (int)sqrt(m + 0.0);
		for (int i = 2; i <= len; i++){
			if (m%i == 0)
				return false;
		}
		return true;
	}

	
	void Ring(int cur){                                   //�ݹ��������������
		
		if (cur == num&&isPrime(a[0] + a[num - 1])){      //�����һ��ִ��,�����ǰ��ý��  
			for (int i = 0; i<num; i++)
				cout << a[i] << ' ';
			cout << endl;
			return;
		}
		
		else for (int i = 2; i <= num; i++){              //ǰn-1��ִ��,�ݹ�ѡ��ÿһ�������,ʹ����ǰһ�������֮��Ϊ����  
			if (!isUsed[i] && isPrime(i + a[cur - 1])){   //��ǰֵiû��ʹ��,����ǰһ��ѡ��ֵ֮��Ϊ����  
				a[cur] = i;                               //ѡiΪ��ǰ��ֵ  
				isUsed[i] = true;                         //״̬��û��ʹ�ø�Ϊ��ʹ��  
				Ring(cur + 1);                            //������һ��
				isUsed[i] = false;                        //�ݹ���������ڴ�n-1�㵽��1��ص�ʱִ��  
				                                          //״̬��ԭ,ʹ��������һ����Ч��ʱ��������  
			}
		}
	}
	void PrintRing(){
		init(a, isUsed);
		cout << "�����������������ָ���(�ɲ��Զ������ݣ�0Ϊ�˳���������С�㣬��������������3��4��8)��";
		cin >> num;
		while (cin >> num, num){
			Ring(1);
		}
		
	}
};

#endif