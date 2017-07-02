#ifndef CACULATOR_H
#define CACULATOR_H

#include <QWidget>

namespace Ui {
class Caculator;
}

class Caculator : public QWidget
{
    Q_OBJECT

public:
    explicit Caculator(QWidget *parent = 0);
    ~Caculator();

private:
    Ui::Caculator *ui;

    bool isOpClicked=false;          //用于判断运算符点击是否有效，‘false’表示未被按下，‘true’表示已被按下
    bool isEqlClicked=false;         //用于判断是否开始新的运算
    QString express="";

private slots:
    void numClickedSlot(void);       //数字
    void opClickedSlot(void);        //操作符 +-*/
    void eqlClickedSlot(void);       // =
    void pointClickedSlot(void);     //点
    void clearClickedSlot(void);     //清屏，清楚结果文本框
    void clearAllClickedSlot(void);  //清屏，清除所有文本框
    void delClickedSlot(void);       //退格处理函数
    void caculateSlot(void);         //表达式计算函数
    void sqrtClickedSlot(void);      //求平方根
    void squareClickedSlot(void);    //求平方
    void threeClickedSlot(void);     //求三次方
    void rcpClickedSlot(void);       //求倒数
    void negativeClickedSlot(void);  //求相反数
};

#endif // CACULATOR_H
