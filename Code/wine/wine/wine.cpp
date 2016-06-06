// wine.cpp : 定义控制台应用程序的入口点。
//

#include "stdafx.h"
#include<iostream>

using namespace std;
#define MAX 10000
int bottle[3];
int time[MAX], q = 0;
int v[10][10][10], k = 0;
int from[MAX], to[MAX], volume[MAX];

int pour(int, int);
void move(int, int, int);
bool whetherStop();
void dfs();

int _tmain(int argc, _TCHAR* argv[])
{
	bottle[0] = 0;                 //初始化容器中的空间
	bottle[1] = 5;
	bottle[2] = 3;
	memset(v, 0, sizeof(v));
	dfs();                         //调用深度优先函数
	int min = time[0];
	for (int m = 0; time[m]; m++){
		
		if (time[m] < min) min = time[m];
		
	}
	cout << "完成分酒任务需要的最佳步骤为：" << endl;
	for (int m = 0; m < min; m++)
		cout << "杯" << from[m] << "倒入杯" << to[m] << "中" << volume[m] << "升酒" << endl;
	cout << "共" << min << "步" << endl;
	
	return 0;
}


int pour(int i, int j)            //判断bottle[i]中的酒可向bottle[j]中倒入多少
{
	int can;
	if (i == j) return 0;
	if (bottle[i] <= 0) return 0;
	if (i == 0)                    //倒酒方式共六种
	{
		if (j == 1)
		{
			can = 5 - bottle[j];   //计算j中的剩余空间
			if (bottle[i]<can)     //返回较小的值为可倒酒量
				return bottle[i];
			else
				return can;
		}
		else if (j == 2)
		{
			can = 3 - bottle[j];
			if (bottle[i]<can)
				return bottle[i];
			else
				return can;
		}
	}
	else if (i == 1)
	{
		if (j == 0)
		{
			can = 8 - bottle[j];
			if (bottle[i]<can)
				return bottle[i];
			else
				return can;
		}
		else if (j == 2)
		{
			can = 3 - bottle[j];
			if (bottle[i]<can)
				return bottle[i];
			else
				return can;
		}
	}
	if (i == 2)
	{
		if (j == 0)
		{
			can = 8 - bottle[j];
			if (bottle[i]<can)
				return bottle[i];
			else
				return can;
		}
		else if (j == 1)
		{
			can = 5 - bottle[j];
			if (bottle[i]<can)
				return bottle[i];
			else
				return can;
		}
	}
}
void move(int i, int j, int can)         //从i倒can升酒到j
{
	bottle[i] -= can;
	bottle[j] += can;
}
bool whetherStop()                      //判断是否已经分好了
{
	if (bottle[0] == 4 && bottle[1] == 4 && bottle[2] == 0)
		return true;
	else return false;
}
void dfs()                              //深度优先搜索
{
	int i, j, can;
	if (whetherStop())                  //如果分酒成功，则输出步骤
	{
		cout << endl;
		//cout << "YES,We Success!!!" << endl << endl;
		time[q++] = k;
		for (int m = 0; m<k; m++)
			cout << "杯" << from[m] << "倒入杯" << to[m] << "中" << volume[m] << "升酒" << endl;
		cout << "共" << k << "步" << endl;
		return;
	}
	else
	{
		for (i = 0; i<3; i++)                     //深度优先遍历
		for (j = 0; j<3; j++)
		{
			if (i != j)
			{
				can = pour(i, j);
				if (can>0 && v[bottle[0]][bottle[1]][bottle[2]] == 0)
				{
					v[bottle[0]][bottle[1]][bottle[2]] = 1; //标志位记录下此时的状态,避免重复判断
					int Eight = bottle[0];
					int Five = bottle[1];
					int Three = bottle[2];
					move(i, j, can);                        //执行倒酒操作
					from[k] = i + 1; to[k] = j + 1; volume[k++] = can;
					dfs();
					k--;
					v[Eight][Five][Three] = 0;
					move(j, i, can);
				}
			}
		}
	}
}
