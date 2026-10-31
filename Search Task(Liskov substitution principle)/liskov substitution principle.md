liskov substitution principle





المبدأ ده بيحكي عن شرط مهم اوي عشان أي class ي inherit من أي class تاني parent



المبدأ بيقول عشان أي class ي inherit من أي parent class لازم ينفذ نفس ال behavior بتاع ال parent بمعنى اصح لما اجي ابدل ال parent ب child المفروض النتائج تبقى زي ما هي



فيه مثال مشهور بيخالف المبدأ ده هو المربع و المستطيل



نتخيل معانا



class Rectangle{



int width;

int height;



void setWidth(int width)

{

this.width = width;

}



void setHeight(int height)

{

this.height = height;

}

}



class Square : Rectangle{

override void setWidth(int width)

{

this.width = width;

this.height = height;

}



override void setHeight(int height)

{

this.width = width;

this.height = height;

}

}



هنا المبرمج يقدر يفكر على ان كل مربع هو مستطيل علاقة "is a" فاذا يعمل inheritance



و هنا المشكلة بتحصل ان المربع بيغير الbehavior بتاع المستطيل ان الطول يساوي العرض عشان كدة لو عملت setwidth للمستطيل ب 4 مثلا و setHeight ب 5 مثلا المتوقع المساحة تكون ب 20



ماذا لو عملت نفس الكلام و نفذت ال2 functions على المربع setHeight الأخيرة هتخلي الطول و العرض ب 5 فالمساحة هتكون ب 25 و دة يعارض مبدأ liskov





مثال تاني عندي مثلا 

class Bird{

void fly()

}

class Eagle : Bird

{

}

class Penguin : Bird

{

}



هنا مفيش مشكلة مع الEagle لانه بالفعل بيطير م بيغيرش parent behavior



انما المشكلة هنا في الPenguin  البطريق في الحقيقة مبيطيرش و بالتالي مبينفذش الparent behavior



من ضمن الحلول لمشكلة زي دي هي اني بعمل class  اسمه FlyBird  بيرث من الBird  وجواه function fly  و اشيل الfly من الBird  الأصلي 



و بالتالي أي طائر بالفعل بيطير يرث من ال FlyBird class  و أي طائر مبيطيرش فعلا يرث من Bird class  وده يعتبر حل من الحلول عشان اطبق مبدأ liskov

