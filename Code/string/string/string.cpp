// string.cpp : 定义控制台应用程序的入口点。
//
///动态规划

#include"stdafx.h"
#include<iostream>

using namespace std;

int lcs_length(char x[], char y[]);

int _tmain(int argc, _TCHAR* argv[])
{
	char x[100], y[100];
	int len;
	while (1)                      //可连续输入多组数据
	{
		cout << "请输入第一个字符串x:";
		cin >> x;
		cout << endl;
		if (x[0] == '0')           //输入0结束
			break;
		cout << "请输入第二个字符串y:";
		cin >> y;
		cout << endl;
		len = lcs_length(x, y);    //调用函数求相同字符串长度
		cout << "最长公共子序列为：";
		cout << len << endl;
		cout << endl;
	}
	return 0;
}
int lcs_length(char x[], char y[])
{
	int m, n, i, j, l[100][100];
	m = strlen(x);
	n = strlen(y);
	for (i = 0; i < m + 1; i++){     //i*j大小的矩阵
		l[i][0] = 0;
	}
	for (j = 0; j < n + 1; j++){
	    l[0][j] = 0;
    }
	for (i = 1; i <= m; i++){      //判断字符串是否相等，计算出矩阵的每个值，后面的值由前面值定
		for (j = 1; j <= n; j++){
			if (x[i - 1] == y[j - 1]) {    //i,j从1开始，但字符串是从0开始，
				l[i][j] = l[i - 1][j - 1] + 1;   //如果对应两字符相等，矩阵对应位置值为前一值加一
			}
			else if (l[i][j - 1]>l[i - 1][j]){    //否则l[i][j]的值为l[i][j-1]与l[i-1][j]中的较大值
				l[i][j] = l[i][j - 1];
			}
			else
				l[i][j] = l[i - 1][j];
		}
	}
	return l[m][n];               //返回矩阵最后一行最后一列的值
}

