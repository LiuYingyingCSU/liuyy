$(document).ready(function () {
    $('#calendar').eCalendar({
        weekDays: ['日', '一', '二', '三', '四', '五', '六'],
		months: ['一月', '二月', '三月', '四月', '五月', '六月','七月', '八月', '九月', '十月', '十一月', '十二月'],
		textArrows: {previous: '<', next: '>'},
		eventTitle: '招聘列表',
		url: '',
		events: [
		    {title: '<a href="http://www.baidu.com" target="_blank">华为深圳工程师</a>', description: '华为集团是中国……', datetime: new Date(2015, 9, 13, 17)},
		    {title: '<a href="http://www.baidu.com" target="_blank">华为深圳工程师</a>', description: '华为集团是中国……', datetime: new Date(2015, 9, 14, 17)},
		    {title: '<a href="http://www.baidu.com" target="_blank">华为深圳工程师</a>', description: '华为集团是中国……', datetime: new Date(2015, 9, 2, 17)},
		    {title: '<a href="http://www.baidu.com" target="_blank">华为深圳工程师</a>', description: '华为集团是中国……', datetime: new Date(2015, 9, 14, 17)}
		]
	});
});



// $(function () {$('#calendar').eCalendar({url: 'loadCalendar',
//                           weekDays: ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat']});

