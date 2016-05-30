// Algorithm.cpp : �������̨Ӧ�ó������ڵ㡣
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
	//int len = sizeof(a) / sizeof(a[0]);  //�������ĳ���
	//First frt;                           //ʵ�����࣬�������к���
	//while (m >= 0)
	//{
	//	cout << "������Ҫ���ҵ�����";
	//	cin >> m;
	//	cout << "ʹ�ö��ֲ��ң�" << "\n";
	//	frt.BinarySearch(m, a,len-1);
	//}

	//frt.QuickSort(b, 0, len-1);            //��������
	//for (int i = 0; i < len; i++){
	//	cout << b[i] << " ";
	//}
	//cout << "\n";

	/////0-1��������
	//PrintPackage(w,v);                   

	///�˻ʺ�����
	Second snd;
	int qnum;
	cout << "������ʺ������";
	cin >> qnum;
	snd.Queen(qnum);

	return 0;
}

void BackTrack(int i, int cv, int cw){         //iΪ��i����Ʒ��cvΪ��ǰ��������Ʒ�ļ�ֵ��cwΪ��ǰ��������Ʒ������
	if (i >= n){                           //�Ѿ����ݽ���
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
				
				cw += w[i] * x[i];        //������Ʒ
				cv += v[i] * x[i];
				BackTrack(i+1, cv, cw);
				cw -= w[i] * x[i];
				cv -= v[i] * x[i];
			}
		}
	}
}
void PrintPackage(int w[], int v[]){

	cout << "0-1����������ݷ��������뱳���������c:";
	cin >> c;
	BackTrack(0, 0, 0);
	cout << "��Ʒ��������ֵ��ѡ�񷽰��ֱ�Ϊ��" << '\n';
	for (int j = 0; j < n; j++){
		cout << w[j] << " " << v[j] << "  " << bestx[j] << '\n';
	}
}

