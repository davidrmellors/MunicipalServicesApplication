   M S B A M L   `   `   `  ����   MunicipalServicesApplication;    5MunicipalServicesApplication.Views.ReportIssuesWindow   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   _ [MaterialDesignThemes.Wpf, Version=5.1.0.0, Culture=neutral, PublicKeyToken=df2a72020bd7962aFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  5   	      MaterialDesignBody#���   5   	   $��Regular"�5   	   $��13(�5   	   &   System.Windows.Media.TextOptions    TextFormattingMode$  Ideal=�5   	      TextRenderingMode$
 Auto=�5	   	     MaterialDesignFont#���  5
   	    � 	Resources 5        PrimaryColor �� Color  SecondaryColor  AccentColor  BackgroundGradient �� 
StartPoint �� EndPoint � Color � Offset��%  &	       &	 !     &	 B     &	 c     �� 5   
   $ #007A4D��6/   �� 5   
   $ #4A4A4A��61   �� 5   
   $ #003399��6.   �� 5   
   $	 0,0&�69   $	 1,1&�6J   .��5      � $ #F0F0F0��6   $ 0Q�6+   � 5      $ #E0E0E0��6   $ 1Q�6+   5      5      .��5      � #��[ 6    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5      $
��Auto �6   .+�5!   
   � $��6��6   	 � ColumnDefinitions	 5"      �� 5#      $��* �6#   �� 5$      $��* �6#   �� 5%      $��* �6#   �� 5&      $��* �6#   �� 5'      $��* �6#   �� 5(      $��* �6#   .+�5*      �� $��0��6   ���#E03C316(   �� 5+      $��1��6   ���#FFFFFF6(   �� 5,      $��2��6   ���#007A4D6(   �� 5-      $��3��6   ���#0014896(   �� 5.      $��4��6   ���#FFB6126(   �� 5/      $��5��6   ���#0000006(   50      � 53   
   $��1��6   
�������$��20,10q�6/   	 54      �� 55      $
��Auto �6#   �� 56      $��* �6#   �� 57      $
��Auto �6#   .+�5:      ]� $��1��6   $��Center=�6(   .+�5;      '  !MaterialDesignThemes.Wpf.PackIcon  
   Kind$
 ClipboardEdit=�6*   #��[ 5<   (   $��32��5=   (   $��32��5>   (   $��	10,0,16,0q�5?   (   $��Center=�5@   (   �� 5A      .+�5B      �� $��Report an Issue��6    $��24(�5C      $
��Bold"�5D      #��[ 5E      �� 5F      $"��Submit a new service request��6    #��[ 5G      $��14(�5H      $��0,4,0,0q�5I      5J      5K      �� 5M      $��2��6   $#��../Resources/zaCoatOfArms.ico��5N      $��60��5O      $��16,10,10,10q�5P      5Q      #  MaterialDesignThemes.Wpf.Card  5T   
   $��2��6   $��20q�6+      UniformCornerRadius$ 8Q�67   .��5U      �� $
��Auto=�6   .��5V      �� $��
40,20,40,0q�6   .+�5W      �� $��Location��6    #��[ 60   $��0,10,0,5q�6]   � 5X      	 5Y      �� 5Z      $��* �6/   �� 5[      $
��Auto �6/   �� 5\      $
��Auto �6/   .+�5^      �� -   5         ��Name$ TxtLocation��5^   "   $��Enter Location��5_   "   #��[ 5`   "    ! MaterialDesignOutlinedTextBox#��[ 5d   "   5e      �� 5f      -   5         ��Name$ LocationSuggestionsList��5f      $	��200��5g      $y�	Collapsed=�5h      $��1q�5j      #��[ 5k      $��0,0,0,10q�5l      ��5m      �� 5n      .R�5o   "   ��  �� Padding$	 8,4q�5p   ,   $
��Wrap=�5q   ,   ��5o   ,   �� *Description+5q   ,   5r      5t      �� 5v      $��Issue Category��6    #��[ 66   $��0,10,0,5q�6c   �� 5x      -   5         ��Name$ CmbCategory��5x      $��0,0,0,15q�64    " MaterialDesignOutlinedComboBox#��[ 5y      )  #MaterialDesignThemes.Wpf.HintAssist   Hint$ Select a category��5z      .@�5|      �� $��
Sanitation��6'   �� 5}      $��Roads��6'   �� 5~      $��	Utilities��6'   �� 5      $��Public Safety��6'   �� 5�      $��Parks and Recreation��6'   �� 5�      $��Public Transportation��6'   �� 5�      $��Noise Complaints��6'   �� 5�      $��Building and Construction��6'   �� 5�      $��Environmental Issues��6'   �� 5�      $��Other��6'   5�      �� 5�      $��Description��6    #��[ 63   $��0,10,0,5q�6`   �� 5�      -   5        $ TxtDescription��5�      $��Enter Description��66   #��[ 6O   #��[ 5�       �� TextWrapping$
 Wrap=�5�       �� AcceptsReturn . $��50��6G   $��0,0,0,15q�6S   �� 5�      $��Attachments��6    #��[ 63   $��0,10,0,5q�6`   �� 5�       �� Orientation$ 
Horizontal=�6!   $��0,0,0,15q�6:   .+�5�      �� -   5        $��Attach Files��5�   !      MaterialDesignOutlinedButton#��[ 5�   !   #��[ 5�   !   #��[ 5�   !   $��0,0,10,0q�5�   !   �� 5�      -   5         ��Name$ TxtAttachmentCount��5�   $   $��Center=�6@   #��[ 5�   $   5�      � 5�      -   5         �Name$ ProgressBar��5�   "    � Minimum$ 0Q�67    � Maximum$	 100Q�6C   $��10��6Q    #	 MaterialDesignLinearProgressBar#��[	 5�   "   #��[ 5�   "   $��0,0,0,15q�5�   "   �� 5�      -   5        $��Submit��5�       
 MaterialDesignRaisedButton#��[
 5�      #��[ 5�      
�������$��0,0,0,15q�5�      5�      5�      5�      �� 5�   
   -	   5        $��3��5�      $��Back to Main Menu��6   #��[ 5�      #��[ 5�      #��[ 5�      $��40,15,40,20q�5�      $��Stretch=�5�      5�      5�      