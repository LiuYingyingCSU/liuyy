#ifndef HEADER_SECOND
#define HEADER_SECOND

#include<iostream>
#include<math.h>
using namespace std;


class Second{
public:
	int x[100];
	bool place(int k)                                      //����ʺ�k������x[k]���Ƿ�����ͻ
	{
		int i;
		for (i = 1; i<k; i++)
		if (x[k] == x[i] || abs(k - i) == abs(x[k] - x[i]))
			return false;                                  //������i-k ==x[i]-x[k]  �������ͻ
		return true;
	}

	void Queen(int n)
	{
		int i, k;
		for (i = 1; i <= n; i++)
			x[i] = 0;
		k = 1;
		while (k >= 1)
		{
			x[k] = x[k] + 1;                                //����һ�з��õ�k���ʺ�
			while (x[k] <= n&&!place(k))                    //�������Ƿ���԰ڷŻʺ�
				x[k] = x[k] + 1;                            //������һ��
			if (x[k] <= n&&k == n)                          //�õ�һ�����
			{
				for (i = 1; i <= n; i++){                   //�Ѱ˻ʺ��λ�ðڷ�ͼ�λ����
					for (int j = 1; j <= n; j++){
						if (j == x[i])
							cout << "O  ";
						else cout << "-  ";

					}
					cout<< '\n';
				}
				cout << "#---------------------#" << '\n';
				return;//��return��ֻ�������һ�ֽ⣬����return����Լ������ݣ����ȫ���Ŀ��ܵĽ�
			}
			else if (x[k] <= n&&k<n)
				k = k + 1;//������һ���ʺ�
			else
			{
				x[k] = 0;//����x[k],����
				k = k - 1;
			}
		}
	}
};



#endif