// Algorithm.cpp : 定义控制台应用程序的入口点。
//

#include "stdafx.h"
#include<iostream>
#include"algorithm.h"
#include"Queen.h"

using namespace std;
#define NUM 5

int c, n = NUM;
int bestv = 0, x[NUM], bestx[NUM];
int w[5] = { 4, 9, 6, 12, 30 }, v[5] = { 12, 20, 16, 12, 100 };
void PrintPackage(int w[],int v[]);

int _tmain(int argc, _TCHAR* argv[])
{
	//int a[NUM] = { 1, 3, 5, 7, 9 }, m = 0;
	//int b[NUM] = { 6, 2, 7, 3, 8};
	//int len = sizeof(a) / sizeof(a[0]);  //获得数组的长度
	//First frt;                           //实例化类，调用类中函数
	//while (m >= 0)
	//{
	//	cout << "请输入要查找的数：";
	//	cin >> m;
	//	cout << "使用二分查找：" << "\n";
	//	frt.BinarySearch(m, a,len-1);
	//}

	//frt.QuickSort(b, 0, len-1);            //快速排序
	//for (int i = 0; i < len; i++){
	//	cout << b[i] << " ";
	//}
	//cout << "\n";

	/////0-1背包问题
	//PrintPackage(w,v);                   

	///八皇后问题
	Second snd;
	int qnum;
	cout << "请输入皇后个数：";
	cin >> qnum;
	snd.Queen(qnum);

	return 0;
}

void BackTrack(int i, int cv, int cw){         //i为第i件物品，cv为当前背包内物品的价值，cw为当前背包内物品的重量
	if (i >= n){                           //已经回溯结束
		if (cv > bestv){
			bestv = cv;
			for (i = 0; i < n; i++) bestx[i] = x[i];
		}
	}
	else{
		for (int j = 0; j <= 1; j++){
			x[i] = j;
			if (cw + x[i] * w[i] <= c){
				/*cout << x[i] << '\n';*/
				
				cw += w[i] * x[i];        //放入物品
				cv += v[i] * x[i];
				BackTrack(i+1, cv, cw);
				cw -= w[i] * x[i];
				cv -= v[i] * x[i];
			}
		}
	}
}
void PrintPackage(int w[], int v[]){

	cout << "0-1背包问题回溯法：请输入背包最大容量c:";
	cin >> c;
	BackTrack(0, 0, 0);
	cout << "物品重量、价值、选择方案分别为：" << '\n';
	for (int j = 0; j < n; j++){
		cout << w[j] << " " << v[j] << "  " << bestx[j] << '\n';
	}
}

