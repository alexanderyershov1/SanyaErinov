   M S B A M L   `   `   `  ����B  >Shebist, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null!clr-namespace:ShebistShebist  5      !    Shebist.AuthorizationWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      'localclr-namespace:Shebist     5      -   5          � Title$  Шебист��5      $
��1366��6+   $	��728��68    � WindowState$ 	Maximized=�6E   .��5
      � 
�������.+�5   
   �� -   5         ��Name$ LoginTextBox��5      $
��Bold"�5      
������� �� TextAlignment$ Center=�5      $��
0, 0, 0, 2q�5      
���   �$��Center=�5      $��22��6/   $��314,90,284,233q�6;    �� TextWrapping$
 Wrap=�6S   $��Center=�6g   $	��120��6�   �� 5   
   -   5         ��Name$ 
LoginLabel��5      $��16(�6$   $
��Bold"�62   
���   �$��
Логин��6W   $��Center=�6g   $��339,54,321,260q�6�   $��Center=�6�   �� 5   
   -   5        $ PasswordLabel��5      $
��Bold"�6'   $��16(�69   
���   �$��Пароль��6[   $��Center=�6l   $��334,123,315,189q�6�   $��Center=�6�   �� 5   
   -   5         ��Name$ LoginButton��5      
���   �$��2q�6:   $
��Bold"�6N   
�������$��
Войти��6s   $��Center=�6�   $��334,225,308,98q�6�   $��Center=�6�   $��76��6�   6�   �� 5   
   -   5        $ NoAccountYet��5      $��12(�5      $
��Bold"�5      
���   �$%��Ещё нет аккаунта?��5      $��Center=�6,   $��322,279,279,40q�6I   $��Center=�6a   �� 5   
   -   5         ��Name$ RememberMeCheckBox��5      
���   �$
��Bold"�5!      $!��Запомнить меня��5"      $��Center=�6,   $��314,198,243,132q�6I   $��Center=�6b   $	��161��6}   5#      �� 5$   
   -   5        $ DataRecoveryLabel��5$      
���   �$
��Bold"�5&      $��12(�5'      $ ��Забыли пароль?��5+      $��Center=�6&   $��327,310,285,9q�6C   $��Center=�6Z   �� 5,   
   -	   5        $ WrongDataLabel��5,      $
��Bold"�6(   $5��/Неверный логин или пароль��6:   
���  ��$��Center=�6o   $��285,252,253,67q�6�   $��Center=�6�   G� 5-   
   -
   5         G�Name$ PasswordBox��5-      
�������$��
0, 0, 0, 2q�6=   
���   �$��Center=�6n   $��Center=�6�   $��305,154,275,164q�6�   $��Center=�6�   $	��120��6  $��20��6  5.   
   5/      