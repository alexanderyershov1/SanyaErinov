   M S B A M L   `   `   `  ����B  >Shebist, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null!clr-namespace:ShebistShebist  5   	   "    Shebist.UserStatisticsWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   'localclr-namespace:Shebist     5   	   -   5          � Title$  Шебист��5   	   $	��728��6   $
��1366��6%   .��5	      �� .+�5
   
   � 
���   �$��50��6"   $	��Top=�6.   $
��1366��6F   .+�5      �� -   5         ��Name$ UserStatisticsLabel��5      $��10, 10, 0, 0q�6o   $��18(�6�   $
��Bold"�6�   
�������.��6�   1/Статистика пользователей6�   �� 5      -   5        $ SentenceEditorLabel��5      $��260, 10, 0, 0q�6o   $��18(�6�   $
��Bold"�6�   
�������.��6�   )'Редактор предложений6�   �� 5      -   5        $ DispatchLabel��5      $��480, 10, 0, 0q�6c   $��18(�6z   $
��Bold"�6�   
�������.��6�   Рассылка6�   �� 5      -   5        $ 	ExitLabel��5      $��580, 10, 0, 0q�6[   $��18(�6r   $
��Bold"�6�   
�������.��6�   
Выход6�   5      &   System.Windows.Controls.DataGrid  5   
   -   5          Name$ UsersDataGrid��5      
�������
���   �$
��Bold"�6Q   $��Center=�6c   $	��352��6�   $��Center=�6�   $	��882��6�    �� Left$ 52��6�   
 �� Top$	 155��6�      Columns 5      0  *System.Windows.Controls.DataGridTextColumn  5      ,  &System.Windows.Controls.DataGridColumn   Header$ 
Логин��6%   1  +System.Windows.Controls.DataGridBoundColumn   Binding 64   ��  �� Path$ login�  5      $ Имя��6%    62   �� $
 name�  5      $ 
Почта��6%    64   �� $ email�  5      $% Дата регистрации��6%    6?   �� $ dateOfRegistration�  5      $! Последний вход��6%    6=   �� $ lastEntrance�  5      $7 1Всего времени в приложении��6%    6I   �� $ totalInTheApp�  5      $ Статус��6%    65   �� $ status�5      �� 5   
   -   5        $ TotalUsersLabel��5      $��16(�6)   $
��Bold"�67   $-��'Всего пользвователей��6I   $	 964��6h   $	 153��6z   �� 5   
   -   5        $ OnlineUsersLabel��5      $��16(�6*   $
��Bold"�68   $��Сейчас онлайн��6J   $	 964��6b   $	 184��6t   �� 5   
   -	   5        $ NewForTodayLabel��5      $��16(�6*   $
��Bold"�68   $$��Новых за сегодня��6J   $	 964��6e   $	 215��6w   �� 5   
   -
   5        $ CameTodayLabel��5      $��16(�6(   $
��Bold"�66   $%��Заходили сегодня��6H   $	 964��6c   $	 246��6u   �� 5   
   -   5        	 ��Name$	 UpdateButton��5      $��Обновить��6B   $
��Bold"�6U   
�������$	 964��6{   $	 306��6�   $��75��6�   5       5!      