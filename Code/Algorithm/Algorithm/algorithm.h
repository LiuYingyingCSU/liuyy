#ifndef HEADER_FIRST
#define HEADER_FIRST

#include<iostream>
using namespace std;
#define NUM 5

class First
{
public:
	void BinarySearch(int m, int a[],int len)
	{
		int low, hig, mid;
		low = 0;
		hig = len;
		mid = (low + hig) / 2;                //�ҵ��ֽ磬�м�ֵ
		bool flag = 0;
		while (!flag)
		{
			if (a[low] == m)                  //�ҵ�
			{
				flag = 1;
				cout << m << "���������ĵ�" << low + 1 << "λ��" << "\n";
			}
			else if (a[hig] == m)
			{
				flag = 1;
				cout << m << "���������ĵ�" << low + 1 << "λ��" << "\n";
			}
			else if (low == hig || low == hig - 1 && hig != m&&low != m)
			{
				flag = 1;
				cout << "δ�ҵ���" << m << "\n";
			}
			else
			{
				mid = (low + hig) / 2;
				if (a[mid]>m)hig = mid;       //�ж�Ҫ��ѯ������midֵ�Ĵ�С�Ƚϣ�������ֵ�󣬲����Ұ��
				else low = mid;               //�����������
			}

		}

	}

    void QuickSort(int a[],int first,int last){

        if (first >= last)                        //���Ѿ�ȫ�����򣬽���
		{
			return;
		}
		int low,hig,key;                    //���������low ,hig,��λ��ǣ�keyΪ�����Ƚϴ�С�ļ�ֵ
		hig = last;
		low = first;
		key = a[first];                        //keyֵ������˴�ѡȡ�����һ����
		while (low < hig)
		{
			while (low<hig&&a[hig] >= key)     //���������һ���������ҵ�һ��С��key����
			{
				--hig;
			}
			a[low] = a[hig];                   //�ҵ�֮��ѽ�Сֵ���ڵͶ�
			//a[hig] = key;
			while (low<hig&&a[low] <= key){    //��ǰ������ҵ�һ������key��ֵ
				low++;
			}
			a[hig] = a[low];                   //�Ѵ�ֵ�ŵ��߶�
			//a[low] = key;
		}
		a[low] = key;                          //һ���������
		QuickSort(a, first, low-1);            //�����߷ֱ���п���
		QuickSort(a, hig+1, last);
	}
	
};

#endif