import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import '../providers/providers.dart';

class EarningsScreen extends StatefulWidget {
  const EarningsScreen({Key? key}) : super(key: key);

  @override
  State<EarningsScreen> createState() => _EarningsScreenState();
}

class _EarningsScreenState extends State<EarningsScreen> {
  String selectedPeriod = 'monthly';

  @override
  void initState() {
    super.initState();
    Future.microtask(() {
      context.read<MessengerProvider>().fetchEarnings();
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Mis Ganancias'),
        backgroundColor: const Color(0xFF667eea),
      ),
      body: Consumer<MessengerProvider>(
        builder: (context, messengerProvider, _) {
          return SingleChildScrollView(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.stretch,
              children: [
                // Earnings summary cards
                Padding(
                  padding: const EdgeInsets.all(16),
                  child: Column(
                    children: [
                      Card(
                        color: const Color(0xFF667eea),
                        child: Padding(
                          padding: const EdgeInsets.all(24),
                          child: Column(
                            children: [
                              const Text(
                                'Ganancias Totales',
                                style: TextStyle(
                                  color: Colors.white,
                                  fontSize: 14,
                                ),
                              ),
                              const SizedBox(height: 8),
                              Text(
                                '\$${messengerProvider.totalEarnings.toStringAsFixed(2)}',
                                style: const TextStyle(
                                  color: Colors.white,
                                  fontSize: 36,
                                  fontWeight: FontWeight.bold,
                                ),
                              ),
                            ],
                          ),
                        ),
                      ),
                      const SizedBox(height: 12),
                      Row(
                        children: [
                          Expanded(
                            child: Card(
                              child: Padding(
                                padding: const EdgeInsets.all(16),
                                child: Column(
                                  children: [
                                    const Text(
                                      'Pendiente de Pago',
                                      style: TextStyle(
                                        color: Color(0xFF666666),
                                        fontSize: 12,
                                      ),
                                    ),
                                    const SizedBox(height: 8),
                                    Text(
                                      '\$${messengerProvider.pendingEarnings.toStringAsFixed(2)}',
                                      style: const TextStyle(
                                        fontSize: 20,
                                        fontWeight: FontWeight.bold,
                                        color: Color(0xFFFFA500),
                                      ),
                                    ),
                                  ],
                                ),
                              ),
                            ),
                          ),
                          const SizedBox(width: 12),
                          Expanded(
                            child: Card(
                              child: Padding(
                                padding: const EdgeInsets.all(16),
                                child: Column(
                                  children: [
                                    const Text(
                                      'Órdenes Completadas',
                                      style: TextStyle(
                                        color: Color(0xFF666666),
                                        fontSize: 12,
                                      ),
                                    ),
                                    const SizedBox(height: 8),
                                    Text(
                                      '${messengerProvider.completedOrders}',
                                      style: const TextStyle(
                                        fontSize: 24,
                                        fontWeight: FontWeight.bold,
                                        color: Color(0xFF4CAF50),
                                      ),
                                    ),
                                  ],
                                ),
                              ),
                            ),
                          ),
                        ],
                      ),
                    ],
                  ),
                ),
                
                // Period filter
                Padding(
                  padding: const EdgeInsets.symmetric(horizontal: 16),
                  child: Row(
                    children: [
                      _PeriodButton(
                        label: 'Semanal',
                        value: 'weekly',
                        isSelected: selectedPeriod == 'weekly',
                        onPressed: () {
                          setState(() => selectedPeriod = 'weekly');
                        },
                      ),
                      const SizedBox(width: 8),
                      _PeriodButton(
                        label: 'Mensual',
                        value: 'monthly',
                        isSelected: selectedPeriod == 'monthly',
                        onPressed: () {
                          setState(() => selectedPeriod = 'monthly');
                        },
                      ),
                      const SizedBox(width: 8),
                      _PeriodButton(
                        label: 'Anual',
                        value: 'yearly',
                        isSelected: selectedPeriod == 'yearly',
                        onPressed: () {
                          setState(() => selectedPeriod = 'yearly');
                        },
                      ),
                    ],
                  ),
                ),
                
                // Transactions list
                const SizedBox(height: 16),
                Padding(
                  padding: const EdgeInsets.symmetric(horizontal: 16),
                  child: const Text(
                    'Transacciones Recientes',
                    style: TextStyle(
                      fontSize: 16,
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                ),
                ListView.builder(
                  shrinkWrap: true,
                  physics: const NeverScrollableScrollPhysics(),
                  itemCount: messengerProvider.transactions.length,
                  itemBuilder: (context, index) {
                    final transaction = messengerProvider.transactions[index];
                    return ListTile(
                      leading: Container(
                        width: 40,
                        height: 40,
                        decoration: BoxDecoration(
                          shape: BoxShape.circle,
                          color: const Color(0xFFF0F0F0),
                        ),
                        child: const Center(
                          child: Icon(
                            Icons.card_giftcard,
                            color: Color(0xFF667eea),
                          ),
                        ),
                      ),
                      title: Text('Orden #${transaction.orderId}'),
                      subtitle: Text(transaction.date),
                      trailing: Text(
                        '+\$${transaction.amount.toStringAsFixed(2)}',
                        style: const TextStyle(
                          fontWeight: FontWeight.bold,
                          color: Color(0xFF4CAF50),
                        ),
                      ),
                    );
                  },
                ),
                
                // Withdraw button
                Padding(
                  padding: const EdgeInsets.all(16),
                  child: ElevatedButton.icon(
                    onPressed: messengerProvider.pendingEarnings > 0
                        ? () {
                            _showWithdrawDialog(context);
                          }
                        : null,
                    icon: const Icon(Icons.account_balance_wallet),
                    label: const Text('Solicitar Retiro'),
                    style: ElevatedButton.styleFrom(
                      backgroundColor: const Color(0xFF667eea),
                      padding: const EdgeInsets.symmetric(vertical: 14),
                    ),
                  ),
                ),
              ],
            ),
          );
        },
      ),
    );
  }

  void _showWithdrawDialog(BuildContext context) {
    showDialog(
      context: context,
      builder: (context) {
        return AlertDialog(
          title: const Text('Solicitar Retiro'),
          content: const Text(
            'Una vez solicitado el retiro, se procesará en 2-3 días hábiles.',
          ),
          actions: [
            TextButton(
              onPressed: () => Navigator.pop(context),
              child: const Text('Cancelar'),
            ),
            ElevatedButton(
              onPressed: () {
                context.read<MessengerProvider>().requestWithdraw();
                Navigator.pop(context);
                ScaffoldMessenger.of(context).showSnackBar(
                  const SnackBar(
                    content: Text('Solicitud de retiro enviada'),
                  ),
                );
              },
              child: const Text('Confirmar'),
            ),
          ],
        );
      },
    );
  }
}

class _PeriodButton extends StatelessWidget {
  final String label;
  final String value;
  final bool isSelected;
  final VoidCallback onPressed;

  const _PeriodButton({
    required this.label,
    required this.value,
    required this.isSelected,
    required this.onPressed,
  });

  @override
  Widget build(BuildContext context) {
    return Expanded(
      child: ElevatedButton(
        onPressed: onPressed,
        style: ElevatedButton.styleFrom(
          backgroundColor: isSelected ? const Color(0xFF667eea) : Colors.white,
          foregroundColor: isSelected ? Colors.white : const Color(0xFF667eea),
          side: BorderSide(
            color: const Color(0xFF667eea),
            width: isSelected ? 0 : 1,
          ),
        ),
        child: Text(label),
      ),
    );
  }
}
