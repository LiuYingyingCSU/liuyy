#ifndef HEADER_SECOND
#define HEADER_SECOND

#include<iostream>
#include<math.h>
using namespace std;


class Second{
public:
	int x[100];
	bool place(int k)                                      //考察皇后k放置在x[k]列是否发生冲突
	{
		int i;
		for (i = 1; i<k; i++)
		if (x[k] == x[i] || abs(k - i) == abs(x[k] - x[i]))
			return false;                                  //若满足i-k ==x[i]-x[k]  则产生冲突
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
			x[k] = x[k] + 1;                                //在下一列放置第k个皇后
			while (x[k] <= n&&!place(k))                    //检查此列是否可以摆放皇后
				x[k] = x[k] + 1;                            //搜索下一列
			if (x[k] <= n&&k == n)                          //得到一个输出
			{
				for (i = 1; i <= n; i++){                   //把八皇后的位置摆放图形化输出
					for (int j = 1; j <= n; j++){
						if (j == x[i])
							cout << "O  ";
						else cout << "-  ";

					}
					cout<< '\n';
				}
				cout << "#---------------------#" << '\n';
				return;//若return则只求出其中一种解，若不return则可以继续回溯，求出全部的可能的解
			}
			else if (x[k] <= n&&k<n)
				k = k + 1;//放置下一个皇后
			else
			{
				x[k] = 0;//重置x[k],回溯
				k = k - 1;
			}
		}
	}
};



#endif