// string.cpp : �������̨Ӧ�ó������ڵ㡣
//
///��̬�滮

#include"stdafx.h"
#include<iostream>

using namespace std;

int lcs_length(char x[], char y[]);

int _tmain(int argc, _TCHAR* argv[])
{
	char x[100], y[100];
	int len;
	while (1)                      //�����������������
	{
		cout << "�������һ���ַ���x:";
		cin >> x;
		cout << endl;
		if (x[0] == '0')           //����0����
			break;
		cout << "������ڶ����ַ���y:";
		cin >> y;
		cout << endl;
		len = lcs_length(x, y);    //���ú�������ͬ�ַ�������
		cout << "�����������Ϊ��";
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
	for (i = 0; i < m + 1; i++){     //i*j��С�ľ���
		l[i][0] = 0;
	}
	for (j = 0; j < n + 1; j++){
	    l[0][j] = 0;
    }
	for (i = 1; i <= m; i++){      //�ж��ַ����Ƿ���ȣ�����������ÿ��ֵ�������ֵ��ǰ��ֵ��
		for (j = 1; j <= n; j++){
			if (x[i - 1] == y[j - 1]) {    //i,j��1��ʼ�����ַ����Ǵ�0��ʼ��
				l[i][j] = l[i - 1][j - 1] + 1;   //�����Ӧ���ַ���ȣ������Ӧλ��ֵΪǰһֵ��һ
			}
			else if (l[i][j - 1]>l[i - 1][j]){    //����l[i][j]��ֵΪl[i][j-1]��l[i-1][j]�еĽϴ�ֵ
				l[i][j] = l[i][j - 1];
			}
			else
				l[i][j] = l[i - 1][j];
		}
	}
	return l[m][n];               //���ؾ������һ�����һ�е�ֵ
}

