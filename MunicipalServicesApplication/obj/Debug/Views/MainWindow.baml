   M S B A M L   `   `   `  ����   MunicipalServicesApplication3    -MunicipalServicesApplication.Views.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$  	Main Menu��5   	   $	��600��6   $	��800��6(    � WindowStartupLocation$ CenterScreen=�5   	   ���#2c3e50 � Icon$ ../Resources/zaFlag.ico��5   	   $	��600��5   	   $	��600��6   .��5      � $��20q�6    � RowDefinitions 5
   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $��* �6   .+�5   
   �� $!��Municipal Services Platform��6   $��28(�67   $
��Bold"�6E   
�������$��Center=�5      $
��0,10q�61   �� 5   
   $	��Top=�6   $��Center=�6-   $	��300��6J   $��1��6V   $��	0,20,0,20q�6c   .+�5      �� -   5         ��Name$ btnReportIssues��5      $��Report an Issue��6.   $
��0,10q�6f   ���#1abc9c
�������$��10q�5   =   $
��Bold"�6J   $	��200��6\   $��Center=�6h   ��5      �� 5       �� 
TargetType" ��6   .�5      ��  �� Property � �� �� Value �#1abc9c67    �� Triggers 5       P� 5!      	 P� Property	 �@J�IsMouseOver
 P� Value
 . . �5"   "   ��  � �� �#16a0856?   5#      5%      5'      �� 5*      -   5        $ 	btnEvents��5*      $$��Local Events and Announcements��6(   {�.  $
��0,10q�6c   ���#95a5a6
�������$��10q�5+   A   $
��Bold"�6N   $	��200��6`   $��Center=�6l   �� 5.      -   5        $ 	btnStatus��5.      $��Service Request Status��6(   {�.  $
��0,10q�6[   ���#95a5a6
�������$��10q�5/   A   $
��Bold"�6N   $	��200��6`   $��Center=�6l   �� 52      -   5        $ btnExit��52      $
��Exit��6&   $
��0,30q�6K   ���#e74c3c
�������$��10q�53   =   $
��Bold"�6J   $	��200��6\   $��Center=�6h   ��54      �� 55      " ��6   .�56      ��  � �� �#e74c3c67    57      P� 58      	 �@J�IsMouseOver
 . . �59   "   ��  � �� �#c0392b6?   5:      5<      5>      5?      �� 5B   
   $��Reported Issues��6   $��18(�6+   $��SemiBold"�69   
�������$
��Left=�5C      $��2��6/   $��	10,0,0,10q�6<   &   System.Windows.Controls.DataGrid  5F   
   -   5          Name$ IssuesDataGrid��5F         AutoGenerateColumns .     HeadersVisibility$ Column=�6G   ���#34495e
�������   CanUserAddRows .  $��2��5G   R   $��10,40,10,10q�6_   $
��Auto=�5H         ColumnWidth5  /System.Windows.Controls.DataGridLengthConverter$ * 6C   $��0q�5I         GridLinesVisibility$
 None=�6'      RowBackground �#2c3e50   AlternatingRowBackground �#34495e   ColumnHeaderStyle 5K      �� 5L      =  7System.Windows.Controls.Primitives.DataGridColumnHeader"  6   .�5M      ��  � �� �#1abc9c63   �� 5N       � ��
 �����63   �� 5O       � ��$
 Bold"�63   �� 5P       � �� �#2c3e5064   �� 5Q       � ��$ 0,0,0,1q�68   �� 5S       � ��$ 5,0,0,0q�60   �� 5W       � ��$
 Left=�6C   5X         RowStyle 5\      �� 5]      )  #System.Windows.Controls.DataGridRow"  6   .�5^      ��  � ��
 �����63   �� 5_       � ��$ Normal"�63   �� 5`       � �� �#2c3e5064   �� 5a       � ��$ 0,0,0,1q�68    5b      P� 5c      	 �@J�IsMouseOver
 . . �5d      ��  � �� �#16a0856;   5e      5g         Columns 5j      0  *System.Windows.Controls.DataGridTextColumn  5k      ,  &System.Windows.Controls.DataGridColumn   Header$ Location��6%      Width$ * 6T   1  +System.Windows.Controls.DataGridBoundColumn   Binding 67   �� *
Location+6T     5l      $ Category��6%   $ * 6T    67   �� *
Category+6T     5m      $ Description��6%   $ * 6Z    6:   �� *Description+6Z   4  .System.Windows.Controls.DataGridTemplateColumn  5p      $ Attachments��6)   $ * 6>      CellTemplate 5q      �� 5r      .R�5s      �� ��6+   �� *Attachments+��5t   "   �� 5u   &   .R�5v   *   �� �� 5w   .   -   5        ��5w   8   �� .�6|   .�5x   2   �� ���#1abc9c��6<   �� 6M   5y   /   5z   +   5{   '   5}      5~      5�      5�      5�       � Triggers 5�      :� 5�   
    :� RoutedEvent$ Window.Loaded��6   .[�5�      �� .��5�      �� .��5�      W�  �� TargetProperty$ Opacity�6&    W� From$ 0X�6J   	 W� To$ 1X�6S    g� Duration$ 0:0:1D�6Z   5�      5�      5�      5�      