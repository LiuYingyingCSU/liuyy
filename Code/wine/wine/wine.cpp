// wine.cpp : �������̨Ӧ�ó������ڵ㡣
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
	bottle[0] = 0;                 //��ʼ�������еĿռ�
	bottle[1] = 5;
	bottle[2] = 3;
	memset(v, 0, sizeof(v));
	dfs();                         //����������Ⱥ���
	int min = time[0];
	for (int m = 0; time[m]; m++){
		
		if (time[m] < min) min = time[m];
		
	}
	cout << "��ɷ־�������Ҫ����Ѳ���Ϊ��" << endl;
	for (int m = 0; m < min; m++)
		cout << "��" << from[m] << "���뱭" << to[m] << "��" << volume[m] << "����" << endl;
	cout << "��" << min << "��" << endl;
	
	return 0;
}


int pour(int i, int j)            //�ж�bottle[i]�еľƿ���bottle[j]�е������
{
	int can;
	if (i == j) return 0;
	if (bottle[i] <= 0) return 0;
	if (i == 0)                    //���Ʒ�ʽ������
	{
		if (j == 1)
		{
			can = 5 - bottle[j];   //����j�е�ʣ��ռ�
			if (bottle[i]<can)     //���ؽ�С��ֵΪ�ɵ�����
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
void move(int i, int j, int can)         //��i��can���Ƶ�j
{
	bottle[i] -= can;
	bottle[j] += can;
}
bool whetherStop()                      //�ж��Ƿ��Ѿ��ֺ���
{
	if (bottle[0] == 4 && bottle[1] == 4 && bottle[2] == 0)
		return true;
	else return false;
}
void dfs()                              //�����������
{
	int i, j, can;
	if (whetherStop())                  //����־Ƴɹ������������
	{
		cout << endl;
		//cout << "YES,We Success!!!" << endl << endl;
		time[q++] = k;
		for (int m = 0; m<k; m++)
			cout << "��" << from[m] << "���뱭" << to[m] << "��" << volume[m] << "����" << endl;
		cout << "��" << k << "��" << endl;
		return;
	}
	else
	{
		for (i = 0; i<3; i++)                     //������ȱ���
		for (j = 0; j<3; j++)
		{
			if (i != j)
			{
				can = pour(i, j);
				if (can>0 && v[bottle[0]][bottle[1]][bottle[2]] == 0)
				{
					v[bottle[0]][bottle[1]][bottle[2]] = 1; //��־λ��¼�´�ʱ��״̬,�����ظ��ж�
					int Eight = bottle[0];
					int Five = bottle[1];
					int Three = bottle[2];
					move(i, j, can);                        //ִ�е��Ʋ���
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
