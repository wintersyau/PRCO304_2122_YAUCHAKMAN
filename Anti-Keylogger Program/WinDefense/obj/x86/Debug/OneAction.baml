   M S B A M L   `   `   `  ����E  AWinDefense, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'clr-namespace:WinDefense
WinDefense  5   	       WinDefense.OneAction   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   *localclr-namespace:WinDefense     5   	     � Title$  	OneAction��5   	   $	��450��6   $	��800��6(    � WindowStartupLocation$ CenterScreen=�64    � Topmost . .��5	      �  � RowDefinitions 5
   
   �� 5      $	��130 �6   �� 5      $��1* �6   �� 5      $��30 �6   �� 5      $��50 �6   .+�5   
   � $��0��6    � ColumnDefinitions 5      �� 5      $	��150 �6#   �� 5      $��1* �6#    5      �� 5      $��1* �6    �� 5      $��1* �6    �� 5      $��1* �6    .+�5      �� $��0��6   $��0��6$   $��Stretch=�61   $��Stretch=�6M   $��5q�6l   
���  ��
�������$��Center=�6�   $��Center=�6�   .��6�   ProcessName5      �� 5!      -   5        $��ProcessName��5!   1   $��1��6   $��0��6$   $��5q�6D   5"      �� 5$      $��0��6   $��1��6$   $��Stretch=�61   $��Stretch=�6M   $��5q�6l   
���  ��
�������$��Center=�6�   $��Center=�6�   .��6�   ProcessPath5&      �� 5(       �� Orientation$ 
Horizontal=�6   $��1��62   $��1��6B   .+�5*      �� -   5        $��ProcessPath��5*      $	��550��6,   $��5q�69   5+      �� 5,      -   5        $��13 5 5 5q�5,      $��5 0 5 0q�6+   .��6N   
OpenPath6X   5-      �� 50      $��0��6   $��2��6$   $��Stretch=�61   $��Stretch=�6M   $��5q�6l   
���  ��
�������$��Center=�6�   $��Center=�6�   .��6�   ProcessAccess52      �� 54      -   5        $��ProcessAccess��54   1   $��1��6   $��2��6$   $��5q�6F   55      57      �� 59   
   -   5        $��Signs��59      $��1��6   5:      �� 5<   
   -   5        $��CurrentSelectStr��5<      $��2��6*   .��68   5=      � 5?   
   $��3��6    5@      �� 5A      $��1* �6#   �� 5B      $��1* �6#   .+�5E      �� -   5        $��5q�5E      $��0��6    
���������5F      �� 5G       �� EndPoint$ 0.5,1&�6*    �� 
StartPoint$ 0.5,0&�6;   .��5H      �  � Color$ 	#FFFF7373��6'   	 � Offset$	 0Q�69   � 5I      $ 	#FFFF1818��6'   $	 1Q�69   5J      .��5K   %   Pass5M      �� 5O      -   5        $��5q�5O      $��1��6    
���������5P      �� 5Q      $ 0.5,1&�6*   $ 0.5,0&�6;   .��5R      � $ 	#FF89E47B��6'   $	 0Q�69   � 5S      $ 	#FF179304��6'   $	 1Q�69   5T      .��5U   %   Kill5W      5X      5Y      5Z      