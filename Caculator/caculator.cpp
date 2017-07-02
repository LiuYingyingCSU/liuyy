#include "caculator.h"
#include "ui_caculator.h"

#include <QPushButton>
#include <QDebug>
#include <QtMath>
#include <QFile>

Caculator::Caculator(QWidget *parent) :
    QWidget(parent),
    ui(new Ui::Caculator)
{
    ui->setupUi(this);

    //引用样式文件
    QString qss;
    QFile qssFile(":/caculator.qss");
    qssFile.open(QFile::ReadOnly);
    if(qssFile.isOpen())
    {
        qss = QLatin1String(qssFile.readAll());
        qApp->setStyleSheet(qss);
        qssFile.close();
    }

    //数字点击事件绑定

    connect(ui->btn0,SIGNAL(clicked()),
            this,SLOT(numClickedSlot()));
    connect(ui->btn1,SIGNAL(clicked()),
            this,SLOT(numClickedSlot()));
    connect(ui->btn2,SIGNAL(clicked()),
            this,SLOT(numClickedSlot()));
    connect(ui->btn3,SIGNAL(clicked()),
            this,SLOT(numClickedSlot()));
    connect(ui->btn4,SIGNAL(clicked()),
            this,SLOT(numClickedSlot()));
    connect(ui->btn5,SIGNAL(clicked()),
            this,SLOT(numClickedSlot()));
    connect(ui->btn6,SIGNAL(clicked()),
            this,SLOT(numClickedSlot()));
    connect(ui->btn7,SIGNAL(clicked()),
            this,SLOT(numClickedSlot()));
    connect(ui->btn8,SIGNAL(clicked()),
            this,SLOT(numClickedSlot()));
    connect(ui->btn9,SIGNAL(clicked()),
            this,SLOT(numClickedSlot()));


    //操作符 + - * / . 事件绑定

    connect(ui->opAddBtn,SIGNAL(clicked()),
            this,SLOT(opClickedSlot()));
    connect(ui->opSubBtn,SIGNAL(clicked()),
            this,SLOT(opClickedSlot()));
    connect(ui->opMulBtn,SIGNAL(clicked()),
            this,SLOT(opClickedSlot()));
    connect(ui->opDivBtn,SIGNAL(clicked()),
            this,SLOT(opClickedSlot()));
    connect(ui->opPointBtn,SIGNAL(clicked()),
            this,SLOT(numClickedSlot()));


    //'='事件绑定
    connect(ui->eqlBtn,SIGNAL(clicked()),
            this,SLOT(eqlClickedSlot()));


    //clear事件绑定
    connect(ui->clearBtn,SIGNAL(clicked()),
            this,SLOT(clearClickedSlot()));
    connect(ui->clearAllBtn,SIGNAL(clicked()),
            this,SLOT(clearAllClickedSlot()));


    //退格事件绑定
    connect(ui->delBtn,SIGNAL(clicked(bool)),
            this,SLOT(delClickedSlot()));


    //单目运算符事件绑定
    connect(ui->sqrtBtn,SIGNAL(clicked(bool)),
            this,SLOT(sqrtClickedSlot()));

    connect(ui->squareBtn,SIGNAL(clicked(bool)),
            this,SLOT(squareClickedSlot()));

    connect(ui->threeBtn,SIGNAL(clicked(bool)),
            this,SLOT(threeClickedSlot()));

    connect(ui->rcpBtn,SIGNAL(clicked(bool)),
            this,SLOT(rcpClickedSlot()));

    connect(ui->negativeBtn,SIGNAL(clicked(bool)),
            this,SLOT(negativeClickedSlot()));
}

Caculator::~Caculator()
{
    delete ui;
}

//0~9点击事件
void Caculator::numClickedSlot(void)
{
    QPushButton* btn = (QPushButton*)sender();
    if(isEqlClicked==true){                                    //是否开始新的运算，且丢弃上次运算结果
        express="";
        ui->screenLdt->setText(btn->text());
        ui->memLdt->setText(btn->text());
        isEqlClicked=false;
    }else{                                                    //设置下层显示框，显示结果
        if(isOpClicked){                                      //若用户在输入新的数字
            ui->screenLdt->setText(btn->text());
        }else{                                                //用户在继续输入数字
            ui->screenLdt->setText(ui->screenLdt->text()+btn->text());
        }
        ui->memLdt->setText(ui->memLdt->text()+btn->text());  //设置上层显示框，实时显示表达式
    }
    isOpClicked=false;
}

// 运算符处理  + - * /
void Caculator::opClickedSlot(void)
{
    isEqlClicked=false;
    if(!isOpClicked){                                         //判断点击是否生效，求得计算结果后继续点击运算符，则保留上次结果继续运算
        if(ui->screenLdt->text().contains("-")){              //判断数字的符号，将其放入表达式中
            express+=ui->screenLdt->text().mid(1);
        }
        else{
            express+=ui->screenLdt->text();
        }
        caculateSlot();                                       //调用表达式计算函数
        isOpClicked=true;
    }
}
// = 事件
void Caculator::eqlClickedSlot(void)
{
    if(isOpClicked||isEqlClicked){                            //若用户上一步操作是 +-*/=  则此次点击不生效

    }
    else{
        if(ui->screenLdt->text().contains("-")){              //处理负数
            express=express+ui->screenLdt->text().mid(1);
        }
        else{
            express+=ui->screenLdt->text();
        }

        caculateSlot();                                       //调用表达式计算函数

        isOpClicked=false;

        //等于号被点击,清除表达式、记忆框
        if(ui->screenLdt->text().contains("-")){              //判断计算结果的符号
            express="neg";
        }else{
            express="";
        }

        //将表达式存入历史记录框中
        QString hisStr = ui->memLdt->text()+"="+ui->screenLdt->text()+"\t\n"+ui->hisTdt->toPlainText();
        ui->hisTdt->setPlainText(hisStr);
        ui->memLdt->setText(ui->screenLdt->text());
        isEqlClicked=true;
    }
}

//求相反数 可连续求反
void Caculator::negativeClickedSlot(void)
{
    QString screenStr = ui->screenLdt->text();
    QString memStr = ui->memLdt->text();
    QString resStr=tr("%1").arg(screenStr.toDouble()*(-1));
    if(ui->screenLdt->text().contains("-")){                  //对负数求反
        if(isEqlClicked){
            express="";
            ui->memLdt->setText(resStr);
            isEqlClicked=false;
        }else{
            express=express.left(express.lastIndexOf("neg"));
            ui->memLdt->setText(memStr.left(memStr.lastIndexOf("("))+resStr);
        }
    }else{                                                    //正数求反
        express+="neg";
        ui->memLdt->setText(memStr.left(memStr.length()-screenStr.length())+" (-"+screenStr+")");
    }
    ui->screenLdt->setText(resStr);
}

// 求平方根
void Caculator::sqrtClickedSlot(void)
{
    QString screenStr = ui->screenLdt->text();
    QString memStr = ui->memLdt->text();
    QString resStr=tr("%1").arg(sqrt(screenStr.toDouble()));
    ui->memLdt->setText(ui->memLdt->text().left(memStr.length()-screenStr.length())+"√"+screenStr);
    ui->screenLdt->setText(resStr);

}

// 求平方
void Caculator::squareClickedSlot(void){
    QString screenStr = ui->screenLdt->text();
    QString memStr = ui->memLdt->text();
    QString resStr=tr("%1").arg(screenStr.toDouble()*screenStr.toDouble());
    ui->memLdt->setText(ui->memLdt->text()+"^2");
    ui->screenLdt->setText(resStr);
}

/*求三次方*/
void Caculator::threeClickedSlot(void)
{
    QString screenStr = ui->screenLdt->text();
    QString resStr=tr("%1").arg(screenStr.toDouble()*screenStr.toDouble()*screenStr.toDouble());
    ui->memLdt->setText(ui->memLdt->text()+"^3");
    ui->screenLdt->setText(resStr);
}

/*求倒数*/
void Caculator::rcpClickedSlot(void)
{
    QString screenStr = ui->screenLdt->text();
    QString memStr = ui->memLdt->text();
    QString resStr=tr("%1").arg(1.0/screenStr.toDouble());
    ui->memLdt->setText(memStr.left(memStr.length()-screenStr.length())+"1/"+screenStr);
    ui->screenLdt->setText(resStr);
}

// . 事件
void Caculator::pointClickedSlot(void)
{
    if(ui->screenLdt->text().contains(".")){                  //判断用户是否已经输入 .  此次点击是否生效

    }else{
        QPushButton* btn = (QPushButton*)sender();

        ui->screenLdt->setText(ui->screenLdt->text()+btn->text()); //设置下层显示框，显示结果
        ui->memLdt->setText(ui->memLdt->text()+btn->text());  //设置上层显示框，实时显示表达式
    }
}

/* 清除历史记录文本框 */
void Caculator::clearClickedSlot(void)
{
    ui->hisTdt->clear();
}
/* 清除所有文本框 */
void Caculator::clearAllClickedSlot(void)
{
    ui->screenLdt->clear();
    ui->memLdt->clear();
    express="";
}
/* 退格处理 */
void Caculator::delClickedSlot(void)
{
    QString memStr=ui->memLdt->text();                        //获取记录框字符串长度
    if(memStr.right(1)=="+"||memStr.right(1)=="-"||memStr.right(1)=="*"||memStr.right(1)=="/"){

    }else{
        memStr=memStr.left(memStr.length()-1);                //截取除最后一个字符的字符串
        ui->memLdt->setText(memStr);                          //将截取后的字符显示

        QString screenStr=ui->screenLdt->text();              //获取字符串长度
        screenStr=screenStr.left(screenStr.length()-1);       //截取除最后一个字符的字符串
        ui->screenLdt->setText(screenStr);                    //将截取后的字符显示
    }
}

//  计算表达式
void Caculator::caculateSlot(void){

    QPushButton* btn = (QPushButton*)sender();

    if(express.indexOf('+')!=-1){                             //加法运算
        int loa=express.indexOf('+');
        int firNum=0;
        int secNum=0;
        if(express.left(loa).contains("neg")){
            firNum=express.left(loa).mid(3).toDouble()*(-1);
        }else {
            firNum=express.left(loa).toDouble();
        }
        if(express.mid(loa+1).contains("neg")){
            secNum=express.mid(loa+1).mid(3).toDouble()*(-1);
        }else{
            secNum=express.mid(loa+1).toDouble();
        }
        ui->screenLdt->setText(QString::number(firNum+secNum));
        express=ui->screenLdt->text();
    }else if(express.indexOf('-')!=-1){                       //减法运算
        int loa=express.indexOf('-');
        int firNum=0;
        int secNum=0;
        if(express.left(loa).contains("neg")){
            firNum=express.left(loa).mid(3).toDouble()*(-1);
        }else {
            firNum=express.left(loa).toDouble();
        }
        if(express.mid(loa+1).contains("neg")){
            secNum=express.mid(loa+1).mid(3).toDouble()*(-1);
        }else{
            secNum=express.mid(loa+1).toDouble();
        }
        ui->screenLdt->setText(tr("%1").arg(firNum-secNum));
        express=ui->screenLdt->text();
    }else if(express.indexOf('*')!=-1){                       //乘法运算
        int loa=express.indexOf('*');
        int firNum=0;
        int secNum=0;
        if(express.left(loa).contains("neg")){
            firNum=express.left(loa).mid(3).toDouble()*(-1.0);
        }else {
            firNum=express.left(loa).toDouble();
        }
        if(express.mid(loa+1).contains("neg")){
            secNum=express.mid(loa+1).mid(3).toDouble()*(-1.0);
        }else{
            secNum=express.mid(loa+1).toDouble();
        }
        ui->screenLdt->setText(tr("%1").arg(firNum*secNum));
        express=ui->screenLdt->text();
    }else if(express.indexOf('/')!=-1){                       //除法运算
        int loa=express.indexOf('/');
        int firNum=0;
        int secNum=0;
        if(express.left(loa).contains("neg")){
            firNum=express.left(loa).mid(3).toDouble()*(-1.0);
        }else {
            firNum=express.left(loa).toDouble();
        }
        if(express.mid(loa+1).contains("neg")){
            secNum=express.mid(loa+1).mid(3).toDouble()*(-1.0);
        }else{
            secNum=express.mid(loa+1).toDouble();
        }
        ui->screenLdt->setText(tr("%1").arg(firNum/secNum));
        express=ui->screenLdt->text();
    }else{

    }

    QString tmp=btn->text();
    if (tmp=="="){
        tmp="";
    }
    ui->memLdt->setText(ui->memLdt->text()+tmp);
    express+=tmp;

}

