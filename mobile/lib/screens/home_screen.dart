import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import '../providers/providers.dart';

class HomeScreen extends StatefulWidget {
  const HomeScreen({Key? key}) : super(key: key);

  @override
  State<HomeScreen> createState() => _HomeScreenState();
}

class _HomeScreenState extends State<HomeScreen> {
  @override
  void initState() {
    super.initState();
    Future.microtask(() {
      context.read<OrderProvider>().fetchAvailableOrders();
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('BadNews - Mensajero'),
        backgroundColor: Colors.purple,
      ),
      body: Consumer<OrderProvider>(
        builder: (context, orderProvider, _) {
          if (orderProvider.isLoading) {
            return const Center(child: CircularProgressIndicator());
          }

          if (orderProvider.availableOrders.isEmpty) {
            return Center(
              child: Column(
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  const Text('No hay encargos disponibles', style: TextStyle(fontSize: 18)),
                  const SizedBox(height: 16),
                  ElevatedButton(
                    onPressed: () => orderProvider.fetchAvailableOrders(),
                    child: const Text('Actualizar'),
                  ),
                ],
              ),
            );
          }

          return RefreshIndicator(
            onRefresh: () => orderProvider.fetchAvailableOrders(),
            child: ListView.builder(
              itemCount: orderProvider.availableOrders.length,
              itemBuilder: (context, index) {
                final order = orderProvider.availableOrders[index];
                return OrderCardWidget(
                  order: order,
                  onAccept: () {
                    // TODO: Implement accept logic
                    ScaffoldMessenger.of(context).showSnackBar(
                      const SnackBar(content: Text('Encargo aceptado')),
                    );
                  },
                );
              },
            ),
          );
        },
      ),
    );
  }
}

class OrderCardWidget extends StatelessWidget {
  final dynamic order;
  final VoidCallback onAccept;

  const OrderCardWidget({
    Key? key,
    required this.order,
    required this.onAccept,
  }) : super(key: key);

  @override
  Widget build(BuildContext context) {
    return Card(
      margin: const EdgeInsets.all(8),
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                if (order.isAnonymous)
                  Container(
                    padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
                    decoration: BoxDecoration(
                      color: Colors.grey[300],
                      borderRadius: BorderRadius.circular(4),
                    ),
                    child: const Text('🔒 Anónimo'),
                  )
                else
                  const SizedBox(),
                Text('\$${order.price} MXN', 
                  style: const TextStyle(fontWeight: FontWeight.bold, fontSize: 16)),
              ],
            ),
            const SizedBox(height: 12),
            Text('Receptor: ${order.recipientName}', 
              style: const TextStyle(fontWeight: FontWeight.bold)),
            Text('Teléfono: ${order.recipientPhone}', 
              style: const TextStyle(color: Colors.grey)),
            const SizedBox(height: 8),
            Container(
              padding: const EdgeInsets.all(8),
              decoration: BoxDecoration(
                color: Colors.grey[100],
                borderRadius: BorderRadius.circular(4),
              ),
              child: Text(order.message, maxLines: 3, overflow: TextOverflow.ellipsis),
            ),
            const SizedBox(height: 12),
            SizedBox(
              width: double.infinity,
              child: ElevatedButton(
                onPressed: onAccept,
                style: ElevatedButton.styleFrom(
                  backgroundColor: Colors.purple,
                ),
                child: const Text('Aceptar Encargo'),
              ),
            ),
          ],
        ),
      ),
    );
  }
}
