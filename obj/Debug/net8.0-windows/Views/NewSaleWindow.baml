   M S B A M L   `   `   `  ����  WpfApp1!    WpfApp1.Views.NewSaleWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$)  #Оформление продажи��5   	   $	��600��6$   $	��800��61    � WindowStartupLocation$ CenterOwner=�5   	   .��5      � $��10q�6    � RowDefinitions 5   
   �� 5      $��* �6   �� 5	      $
��Auto �6   �� 5
      $	��200 �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   .+�5   
   �� $��0��6   $%��Доступные товары��6    .��5      &   System.Windows.Controls.DataGrid  -   5          Name$ 
dgProducts��5         AutoGenerateColumns$ False��5         
IsReadOnly$
 True��63      SelectionMode$ Single=�5         Columns 5      0  *System.Windows.Controls.DataGridTextColumn  5      ,  &System.Windows.Controls.DataGridColumn   Header$ ID��6)   	   Width5  /System.Windows.Controls.DataGridLengthConverter$	 50 6L   1  +System.Windows.Controls.DataGridBoundColumn
   Binding
 65   �� *Id+6L     5      $ Название��6)   $		 200 6T   
 6;   �� *Name+6T     5      $ Цена��6)   $		 100 6l   
 67   �� *Price+ �� StringFormat$ 
{0:N2} ₽��6l     5      $ В наличии��6)   $		 100 6^   
 6<   �� *StockQuantity+6^   5      5      � 5   
   $��1��6   $
��0,10q�6    � ColumnDefinitions 5      �� 5      $
��Auto �6#   �� 5      $	��100 �6#   �� 5       $
��Auto �6#   .+�5#      �� $��0��6   $��Количество:��6$   �� 5$      -   5         ��Name$ txtQuantity��5$   &   $��1��6   $	��5,0q�6;   �� 5%      -   5         ��Name$ btnAddToCart��5%   %   $��2��6   $(��"Добавить в корзину��6;   $	��5,0q�5&      $
��10,5q�6"   61   5'      �� 5)   
   $��2��6   $��Корзина��6    .��5*        -   5        $ dgCart��5*      $ False��5+      $
 True��63    5,        5-      $ Название��6)   $		 200 6\   
 6;   �� *Product.Name+6\     5.      $ Цена��6)   $		 100 6t   
 67   �� *Product.Price+$ 
{0:N2} ₽��6t     5/      $ Количество��6)   $		 100 6Z   
 6=   �� *
Quantity+6Z     50      $ 
Сумма��6)   $		 100 6m   
 68   �� *Total+$ 
{0:N2} ₽��6m   4  .System.Windows.Controls.DataGridTemplateColumn  51      $
	 Auto 6-      CellTemplate 52      �� 53      .R�54   "   �� -   5        $��Удалить��54   )   6;   55      57      59      5:      �� 5<   
   -   5         ��Name$ txtTotal��5<   !   $��3��6   $��Итого: 0 ₽��63   $��16(�5=      $
��Bold"�6"   $
��0,10q�5>      $��Right=�6"   �� 5@   
   $��4��6    �� Orientation$ 
Horizontal=�6"   $��Right=�6;   .+�5A      �� -   5        $ btnComplete��5A      $��Оформить��6*   $	��100��6=   $��5q�5B      6    �� 5C      -   5        $ 	btnCancel��5C      $��Отмена��6(   $	��100��69   $��5q�5D      6    5E      5F      5G      