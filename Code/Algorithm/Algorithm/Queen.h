#ifndef HEADER_SECOND
#define HEADER_SECOND

#include<iostream>
#include<math.h>
using namespace std;


class Second{
public:
	int x[100];
	bool place(int k)//����ʺ�k������x[k]���Ƿ�����ͻ
	{
		int i;
		for (i = 1; i<k; i++)
		if (x[k] == x[i] || abs(k - i) == abs(x[k] - x[i]))
			return false;
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
			x[k] = x[k] + 1;   //����һ�з��õ�k���ʺ�
			while (x[k] <= n&&!place(k))
				x[k] = x[k] + 1;//������һ��
			if (x[k] <= n&&k == n)//�õ�һ�����
			{
				for (i = 1; i <= n; i++){
					for (int j = 1; j <= n; j++){
						if (j == x[i])
							cout << "1  ";
						else cout << "0  ";

					}
					cout << '\n';
				}
				
					/*printf("%d ", x[i]);
				printf("\n");*/
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