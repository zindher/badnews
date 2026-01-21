import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'providers/providers.dart';
import 'screens/home_screen.dart';

void main() {
  runApp(const BadNewsApp());
}

class BadNewsApp extends StatelessWidget {
  const BadNewsApp({Key? key}) : super(key: key);

  @override
  Widget build(BuildContext context) {
    return MultiProvider(
      providers: [
        ChangeNotifierProvider(create: (_) => OrderProvider()),
        ChangeNotifierProvider(create: (_) => MessengerProvider()),
      ],
      child: MaterialApp(
        title: 'BadNews - Messenger',
        theme: ThemeData(
          primarySwatch: Colors.purple,
          useMaterial3: true,
          appBarTheme: const AppBarTheme(
            backgroundColor: Colors.purple,
            elevation: 0,
          ),
          elevatedButtonTheme: ElevatedButtonThemeData(
            style: ElevatedButton.styleFrom(
              backgroundColor: Colors.purple,
              padding: const EdgeInsets.symmetric(vertical: 12, horizontal: 24),
              shape: RoundedRectangleBorder(
                borderRadius: BorderRadius.circular(8),
              ),
            ),
          ),
        ),
        debugShowCheckedModeBanner: false,
        home: const HomeScreen(),
      ),
    );
  }
}

